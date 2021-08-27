import * as pulumi from "@pulumi/pulumi";
import * as azure from "@pulumi/azure-native";
import * as random from "@pulumi/random";
import { getPublicIp } from "./local-ip";
import { authorization } from "@pulumi/azure-native/types/enums";

const config = new pulumi.Config();
const shouldDeploy = config.requireBoolean("deploy");

const azureAdTenantName = config.require("azure-ad-b2c-tenant-name");
const azureAdTenantId = config.require("azure-ad-b2c-tenant-id");
const azureAdPolicyName = config.require("azure-ad-b2c-policy-name");
const azureAdBackofficeApp = config.require("azure-ad-b2c-backoffice-app");
const azureAdApiApp = config.require("azure-ad-b2c-api-app");
const azureAdApiAppSecret = config.requireSecret("azure-ad-b2c-api-app-secret");

const stack = pulumi.getStack();
const baseName = `tix-${stack}`;
const sqlAdminUser = "tixadmin";

const mainLocation = 'West Europe';

export = async () => {

    const publicIp = await getPublicIp();

    // Create an Azure Resource Group
    const resourceGroup = new azure.resources.ResourceGroup(`${baseName}-group`, { location: mainLocation });

    // Create an Azure resource (Storage Account)
    const storageAccount = new azure.storage.StorageAccount("storage", {
        // The location for the storage account will be derived automatically from the resource group.
        resourceGroupName: resourceGroup.name,
        kind: azure.storage.Kind.StorageV2,
        sku: {
            name: azure.storage.SkuName.Standard_LRS
        }
    });

    const activityViewTable = new azure.storage.Table("activityview", {
        resourceGroupName: resourceGroup.name,
        accountName: storageAccount.name,
        tableName: "activityview",
    });

    //
    // Databases
    //

    // Generate a random password
    const sqlAdminPassword = new random.RandomPassword("tixdbpassword", {
        length: 18,
        special: false,
        minNumeric:1,
        minUpper:1,
        minLower:1
    });

    const sqlServer = new azure.sql.Server("tixdb", {
        resourceGroupName: resourceGroup.name,
        version: "12.0",
        administratorLogin: sqlAdminUser,
        administratorLoginPassword: sqlAdminPassword.result,
    });

    const allAzureFirewallRule = new azure.sql.FirewallRule("tix-fw", {
        resourceGroupName: resourceGroup.name,
        serverName: sqlServer.name,
        startIpAddress: "0.0.0.0",
        endIpAddress: "0.0.0.0"
    });

    const localFirewallRule = new azure.sql.FirewallRule("tix-fw-local", {
        resourceGroupName: resourceGroup.name,
        serverName: sqlServer.name,
        startIpAddress: publicIp ?? "0.0.0.0",
        endIpAddress: publicIp ?? "0.0.0.0"
    });

    const activityEventStoreDatabase = new azure.sql.Database("activityevents", {
        resourceGroupName: resourceGroup.name,
        serverName: sqlServer.name,
    });

    const orderEventStoreDatabase = new azure.sql.Database("orderevents", {
        resourceGroupName: resourceGroup.name,
        serverName: sqlServer.name,
    });

    const processEventStoreDatabase = new azure.sql.Database("processevents", {
        resourceGroupName: resourceGroup.name,
        serverName: sqlServer.name,
    });

    const tixViewsDatabase = new azure.sql.Database("tixviews", {
        resourceGroupName: resourceGroup.name,
        serverName: sqlServer.name,
    });

    //
    // Service bus
    //

    const serviceBusNamespace = new azure.servicebus.Namespace(`${baseName}-bus`, {
        resourceGroupName: resourceGroup.name,
        sku: { 
            name: azure.servicebus.SkuName.Standard
        }
    });

    const activityCommandsQueue = new azure.servicebus.Queue('activitycommands', {
        namespaceName: serviceBusNamespace.name,
        resourceGroupName: resourceGroup.name,
        queueName: 'activitycommands'
    });

    const orderCommandsQueue = new azure.servicebus.Queue('ordercommands', {
        namespaceName: serviceBusNamespace.name,
        resourceGroupName: resourceGroup.name,
        queueName: 'ordercommands'
    });

    const serviceBusAuthRule = new azure.servicebus.NamespaceAuthorizationRule(`${baseName}-bus-rule`, {
        namespaceName: serviceBusNamespace.name,
        resourceGroupName: resourceGroup.name,
        rights: [
            "Listen",
            "Send"
        ]
    });

    //
    // Redis Cache
    //
    const redisCache = new azure.cache.Redis('cmds', {
        resourceGroupName: resourceGroup.name,
        sku: {
            name: "Basic",
            family: azure.cache.SkuFamily.C,
            capacity: 0
        }
    }, {
        customTimeouts: { 
            create: '40m',
            update: '40m',
            delete: '40m'
        }
    });

    const redisCacheKeys = pulumi.all([resourceGroup.name, redisCache.name]).apply(([resourceGroupName, name]) => {
        const keys = azure.cache.listRedisKeys({
            resourceGroupName, name
        });
        return keys;
    });

    //
    // SignalR
    //
/*    const signalR = new azure.signalr.Service('signalr', {
        resourceGroupName: resourceGroup.name,
        sku: {
            name: 'Free_F1',
            capacity: 1
        },
        cors: [{
            allowedOrigins: ['*']
        }],
    });
*/
    //
    // Event Hub
    //
    const eventHubNamespace = new azure.eventhub.Namespace('tixhub', {
        resourceGroupName: resourceGroup.name,
        sku: {
            name: azure.eventhub.SkuName.Standard,
            capacity: 1,
        }
    });

    const eventHubAuthRule = new azure.eventhub.NamespaceAuthorizationRule('tixhub-authrule', {
        resourceGroupName: resourceGroup.name,
        namespaceName: eventHubNamespace.name,
        rights: [
            azure.eventhub.AccessRights.Listen,
            azure.eventhub.AccessRights.Send
        ]
    });

    const activityEventHub = new azure.eventhub.EventHub('activity', {
        resourceGroupName: resourceGroup.name,
        namespaceName: eventHubNamespace.name,
        partitionCount: 32,
        messageRetentionInDays: 1,
        eventHubName: 'activity'
    });

    const activityViewsConsumerGroup = new azure.eventhub.ConsumerGroup('activityviews', {
        resourceGroupName: resourceGroup.name,
        namespaceName: eventHubNamespace.name,
        eventHubName: activityEventHub.name,
        consumerGroupName: 'activityviews'
    });

    const activityProcessConsumerGroup = new azure.eventhub.ConsumerGroup('activityproc', {
        resourceGroupName: resourceGroup.name,
        namespaceName: eventHubNamespace.name,
        eventHubName: activityEventHub.name,
        consumerGroupName: 'activityproc'
    });

    const orderEventHub = new azure.eventhub.EventHub('order', {
        resourceGroupName: resourceGroup.name,
        namespaceName: eventHubNamespace.name,
        partitionCount: 32,
        messageRetentionInDays: 1,
        eventHubName: 'order'
    });

    const orderViewsConsumerGroup = new azure.eventhub.ConsumerGroup('orderviews', {
        resourceGroupName: resourceGroup.name,
        namespaceName: eventHubNamespace.name,
        eventHubName: orderEventHub.name,
        consumerGroupName: 'orderviews'
    });

    const orderProcessConsumerGroup = new azure.eventhub.ConsumerGroup('orderproc', {
        resourceGroupName: resourceGroup.name,
        namespaceName: eventHubNamespace.name,
        eventHubName: orderEventHub.name,
        consumerGroupName: 'orderproc'
    });

    //
    // App Insights
    //
    const appInsights = new azure.insights.Component(`${baseName}-ai`, {
        resourceGroupName: resourceGroup.name,
        kind: azure.insights.Kind.Shared,
        applicationType: azure.insights.ApplicationType.Web,
    });

    const apiTokenSecurityKey = new random.RandomPassword("apitokenkey", {
        length: 32,
        special: false,
        minNumeric:1,
        minUpper:1,
        minLower:1
    });

    // Collect settings
    const activityEventStoreConnection = pulumi.all([sqlServer.name, activityEventStoreDatabase.name, sqlAdminPassword.result]).apply(([server, db, pwd]) =>
        `Server=tcp:${server}.database.windows.net,1433;Initial Catalog=${db};User ID=${sqlAdminUser};password=${pwd};Persist Security Info=False;Encrypt=True;MultipleActiveResultSets=False;Connection Timeout=30;`);
    const orderEventStoreConnection = pulumi.all([sqlServer.name, orderEventStoreDatabase.name, sqlAdminPassword.result]).apply(([server, db, pwd]) =>
        `Server=tcp:${server}.database.windows.net,1433;Initial Catalog=${db};User ID=${sqlAdminUser};password=${pwd};Persist Security Info=False;Encrypt=True;MultipleActiveResultSets=False;Connection Timeout=30;`);
    const processEventStoreConnection = pulumi.all([sqlServer.name, processEventStoreDatabase.name, sqlAdminPassword.result]).apply(([server, db, pwd]) =>
        `Server=tcp:${server}.database.windows.net,1433;Initial Catalog=${db};User ID=${sqlAdminUser};password=${pwd};Persist Security Info=False;Encrypt=True;MultipleActiveResultSets=False;Connection Timeout=30;`);
    const tixViewsConnection = pulumi.all([sqlServer.name, tixViewsDatabase.name, sqlAdminPassword.result]).apply(([server, db, pwd]) =>
        `Server=tcp:${server}.database.windows.net,1433;Initial Catalog=${db};User ID=${sqlAdminUser};password=${pwd};Persist Security Info=False;Encrypt=True;MultipleActiveResultSets=False;Connection Timeout=30;`);

    const primaryServiceBusKey = await pulumi.all([resourceGroup.name, serviceBusNamespace.name, serviceBusAuthRule.name]).apply(async ([resourceGroupName, namespaceName, authRuleName]) => {
        const keys = await azure.servicebus.listNamespaceKeys({ 
            resourceGroupName: resourceGroupName, 
            namespaceName: namespaceName,
            authorizationRuleName: authRuleName
        });
        return keys.primaryConnectionString;
    });

    const storageAccountKey = await pulumi.all([resourceGroup.name, storageAccount.name]).apply(async ([resourceGroupName, accountName]) => {
        const keys = await azure.storage.listStorageAccountKeys({
            resourceGroupName, accountName
        });
        return keys.keys[0].value;
    });

    const eventHubConnectionString = await pulumi.all([resourceGroup.name, eventHubNamespace.name, eventHubAuthRule.name]).apply(async ([resourceGroupName, namespaceName, authorizationRuleName]) => {
        const keys = await azure.eventhub.listNamespaceKeys({
            resourceGroupName, authorizationRuleName, namespaceName
        });
        return keys.primaryConnectionString;
    });

    const activityAppSettings = {
        runtime: "dotnet",
        TixServiceBus: primaryServiceBusKey,
        "APPINSIGHTS_INSTRUMENTATIONKEY": appInsights.instrumentationKey,
        ActivityEventsDbConnection: activityEventStoreConnection,
        ViewsDbConnection: tixViewsConnection,
        AzureWebJobsStorage: storageAccountKey,
        EventHubConnectionString: eventHubConnectionString,
        CommandLogCache: redisCacheKeys.primaryKey,
    };
    const orderAppSettings = {
        runtime: "dotnet",
        TixServiceBus: primaryServiceBusKey,
        "APPINSIGHTS_INSTRUMENTATIONKEY": appInsights.instrumentationKey,
        OrderEventsDbConnection: orderEventStoreConnection,
        ViewsDbConnection: tixViewsConnection,
        AzureWebJobsStorage: storageAccountKey,
        EventHubConnectionString: eventHubConnectionString,
        CommandLogCache: redisCacheKeys.primaryKey,
    };
    const processAppSettings = {
        runtime: "dotnet",
        TixServiceBus: primaryServiceBusKey,
        "APPINSIGHTS_INSTRUMENTATIONKEY": appInsights.instrumentationKey,
        AzureWebJobsStorage: storageAccountKey,
        ProcessEventsDbConnection: processEventStoreConnection,
        EventHubConnectionString: eventHubConnectionString,
    };
    const apiAppSettings = {
        runtime: "dotnet",
        "APPINSIGHTS_INSTRUMENTATIONKEY": appInsights.instrumentationKey,
        AzureWebJobsStorage: storageAccountKey,
        TixServiceBus: primaryServiceBusKey,
        ViewsDbConnection: tixViewsConnection,
        //SignalRConnection: signalR.primaryConnectionString,
        //SignalRHost: signalR.hostname,
        CommandLogCache: redisCacheKeys.primaryKey,
        "AzureAdB2C:Instance": `https://${azureAdTenantName}.b2clogin.com/`,
        "AzureAdB2C:Domain": `${azureAdTenantName}.onmicrosoft.com`,
        "AzureAdB2C:SignUpSignInPolicyId": azureAdPolicyName,
        "AzureAdB2C:ClientId": azureAdApiApp,
        "AzureAdB2C:ClientSecret": azureAdApiAppSecret,
        "AzureAdB2C:TokenValidationParameters:NameClaimType": "name",
        "IssuerIdentifier": `https://${azureAdTenantName}.b2clogin.com/${azureAdTenantId}/v2.0/`,
        "ApiTokenSecurityKey": apiTokenSecurityKey.result,
    };

    let activityAppName: pulumi.Output<string> | undefined;
    let orderAppName: pulumi.Output<string> | undefined;
    let processAppName: pulumi.Output<string> | undefined;
    let apiAppName: pulumi.Output<string> | undefined;
    let apiHostname: pulumi.Output<string> | undefined;
    
    let frontendResourceGroupName: pulumi.Output<string> | undefined;
    let frontpageAppName: pulumi.Output<string> | undefined;
    let frontpageHostname: pulumi.Output<string> | undefined;
    let backOfficeAppName: pulumi.Output<string> | undefined;
    let backOfficeHostname: pulumi.Output<string> | undefined;

    if (shouldDeploy) {
        //
        // Azure Functions apps
        //

        const backendServicePlan = new azure.web.AppServicePlan(`be-plan`, {
            resourceGroupName: resourceGroup.name,
            sku: {
                tier: 'Dynamic',
                size: 'Y1',
            },
            kind: 'FunctionApp'
        });

        const activityAppFunc = new azure.web.WebApp(`${baseName}-activity`, {
            resourceGroupName: resourceGroup.name,
            serverFarmId: backendServicePlan.id,
            kind: 'functionapp',
            siteConfig: {
                appSettings: toNameValuePairs(activityAppSettings),
                http20Enabled: true,
            },            
        });
        activityAppName = activityAppFunc.name;

        const orderAppFunc = new azure.web.WebApp(`${baseName}-order`, {
            resourceGroupName: resourceGroup.name,
            serverFarmId: backendServicePlan.id,
            kind: 'functionapp',
            siteConfig: {
                appSettings: toNameValuePairs(orderAppSettings),
                http20Enabled: true
            }
        });
        orderAppName = orderAppFunc.name;

        const processAppFunc = new azure.web.WebApp(`${baseName}-process`, {
            resourceGroupName: resourceGroup.name,
            serverFarmId: backendServicePlan.id,
            kind: 'functionapp',
            siteConfig: {
                appSettings: toNameValuePairs(processAppSettings),
                http20Enabled: true
            }
        });
        processAppName = processAppFunc.name;

        const apiAppFunc = new azure.web.WebApp(`${baseName}-api`, {
            resourceGroupName: resourceGroup.name,
            serverFarmId: backendServicePlan.id,
            kind: 'functionapp',
            siteConfig: {
                cors: { allowedOrigins: ["*"] },
                appSettings: toNameValuePairs(apiAppSettings),
                http20Enabled: true
            },
        });
        apiAppName = apiAppFunc.name;
        apiHostname = apiAppFunc.defaultHostName;

        //
        // Frontend Apps
        //

        // We need a separate resource group if we want to use a Linux AppService Plan
        const frontendResourceGroup = new azure.resources.ResourceGroup(`${baseName}-fe-group`, { location: mainLocation });
        frontendResourceGroupName = frontendResourceGroup.name;

        const frontendServicePlan = new azure.web.AppServicePlan("fe-plan", {
            resourceGroupName: frontendResourceGroup.name,
            sku: {
                tier: 'Premium',
                size: 'P1v2'
            },
            kind: 'Linux',
            reserved: true,
        });

        const frontpageApp = new azure.web.WebApp(`${baseName}-frontpage`, {
            resourceGroupName: frontendResourceGroup.name,
            serverFarmId: frontendServicePlan.id,
            siteConfig: {
                linuxFxVersion: 'NODE|12-lts',
                appSettings: [
                    { name: "APPINSIGHTS_INSTRUMENTATIONKEY", value: appInsights.instrumentationKey },
                    { name: "APPLICATIONINSIGHTS_CONNECTION_STRING", value: pulumi.interpolate `InstrumentationKey=${appInsights.instrumentationKey}` },
                    { name: "ApplicationInsightsAgent_EXTENSION_VERSION", value: "~2" },
                    { name: "SCM_DO_BUILD_DURING_DEPLOYMENT", value: "true" },
                    { name: "WEBSITE_NODE_DEFAULT_VERSION", value: "12.18.0" },
                    { name: "NEXT_PUBLIC_API_ROOT", value: pulumi.interpolate `https://${apiHostname}/api` },
                    { name: "NEXT_PUBLIC_APPINSIGHTS_INSTRUMENTATIONKEY", value: appInsights.instrumentationKey },
                ],
            },
        });

        const backOfficeApp = new azure.web.WebApp(`${baseName}-backoffice`, {
            resourceGroupName: frontendResourceGroup.name,
            serverFarmId: frontendServicePlan.id,
            siteConfig: {
                linuxFxVersion: 'NODE|12-lts',
                appSettings: [
                    { name: "APPINSIGHTS_INSTRUMENTATIONKEY", value: appInsights.instrumentationKey },
                    { name: "APPLICATIONINSIGHTS_CONNECTION_STRING", value: pulumi.interpolate `InstrumentationKey=${appInsights.instrumentationKey}` },
                    { name: "ApplicationInsightsAgent_EXTENSION_VERSION", value: "~2" }, 
                    { name: "SCM_DO_BUILD_DURING_DEPLOYMENT", value: "true" },
                    { name: "WEBSITE_NODE_DEFAULT_VERSION", value: "12.18.0" },
                    { name: "NEXT_PUBLIC_API_ROOT", value: pulumi.interpolate `https://${apiHostname}/api` },
                    { name: "NEXT_PUBLIC_FRONTPAGE_ROOT", value: pulumi.interpolate `https://${frontpageApp.defaultHostName}` },
                    { name: "NEXT_PUBLIC_APPINSIGHTS_INSTRUMENTATIONKEY", value: appInsights.instrumentationKey },
                    { name: "NEXT_PUBLIC_AUTH_TENANT_NAME", value: azureAdTenantName },
                    { name: "NEXT_PUBLIC_AUTH_CLIENT_ID", value: azureAdBackofficeApp },
                    { name: "NEXT_PUBLIC_USER_FLOW", value: azureAdPolicyName }
                ],
            },
        });
        
        backOfficeHostname = backOfficeApp.defaultHostName;
        frontpageHostname = frontpageApp.defaultHostName;
        backOfficeAppName = backOfficeApp.name;
        frontpageAppName = frontpageApp.name;
    }

    const output = {
        // Return some connection strings
        storageConnectionString: storageAccountKey,
        serviceBusConnectionString: primaryServiceBusKey,
        activityEventStoreConnectionString: activityEventStoreConnection,
        orderEventStoreConnectionString: orderEventStoreConnection,
        tixViewsConnectionString: tixViewsConnection,
        apiHostname: apiHostname,
        backOfficeHostname: backOfficeHostname,
        frontpageHostname: frontpageHostname
    };

    return {
        out: output,
        frontendSettings: {
            frontendResourceGroupName: frontendResourceGroupName,
            backOfficeAppName: backOfficeAppName,
            frontpageAppName: frontpageAppName,
        },
        backendSettings: {
            backendResourceGroupName: resourceGroup.name,
            activityAppName,
            orderAppName,
            processAppName,
            apiAppName,
        },
        activityAppSettings: {
            IsEncrypted: false,
            Values: {
                ...activityAppSettings,
                "AzureFunctionsJobHost__logging__logLevel__default": "Information",
            }
        },
        orderAppSettings: {
            IsEncrypted: false,
            Values: {
                ...orderAppSettings,
                "AzureFunctionsJobHost__logging__logLevel__default": "Information",
            }
        },
        processAppSettings: {
            IsEncrypted: false,
            Values: {
                ...processAppSettings,
                "AzureFunctionsJobHost__logging__logLevel__default": "Information",
            }
        },
        apiAppSettings: {
            IsEncrypted: false,
            Values: {
                ...apiAppSettings,
                "AzureFunctionsJobHost__logging__logLevel__default": "Information",
            },
            Host: {
                CORS: "*"
            }
        }
    };
}

function toNameValuePairs(o: any) {
    const list = [];
    for (const key in o) {
        if (Object.prototype.hasOwnProperty.call(o, key)) {
            const value = o[key];
            list.push({name: key, value: value});
        }
    }
    return list;
}