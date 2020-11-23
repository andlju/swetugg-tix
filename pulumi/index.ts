import * as pulumi from "@pulumi/pulumi";
import * as azure from "@pulumi/azure";
import * as random from "@pulumi/random";
import { getPublicIp } from "./local-ip";
import { FailoverGroup } from "@pulumi/azure/sql";

const mainLocation = azure.Locations.NorthEurope;

const config = new pulumi.Config();
const shouldDeploy = config.requireBoolean("deploy");

const stack = pulumi.getStack();
const baseName = `tix-${stack}`;
const sqlAdminUser = "tixadmin";

export = async () => {

    const publicIp = await getPublicIp();

    // Create an Azure Resource Group
    const resourceGroup = new azure.core.ResourceGroup(`${baseName}-group`, { location: mainLocation });

    // Create an Azure resource (Storage Account)
    const storageAccount = new azure.storage.Account("storage", {
        // The location for the storage account will be derived automatically from the resource group.
        resourceGroupName: resourceGroup.name,
        accountTier: "Standard",
        accountReplicationType: "LRS",
    });

    const activityViewTable = new azure.storage.Table("activityview", {
        storageAccountName: storageAccount.name,
        name: "activityview",
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

    const sqlServer = new azure.sql.SqlServer("tixdb", {
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
        requestedServiceObjectiveName: "S0",
    });

    const orderEventStoreDatabase = new azure.sql.Database("orderevents", {
        resourceGroupName: resourceGroup.name,
        serverName: sqlServer.name,
        requestedServiceObjectiveName: "S0",
    });

    const processEventStoreDatabase = new azure.sql.Database("processevents", {
        resourceGroupName: resourceGroup.name,
        serverName: sqlServer.name,
        requestedServiceObjectiveName: "S0",
    });

    const tixViewsDatabase = new azure.sql.Database("tixviews", {
        resourceGroupName: resourceGroup.name,
        serverName: sqlServer.name,
        requestedServiceObjectiveName: "S0",
    });

    //
    // Service bus
    //

    const serviceBusNamespace = new azure.servicebus.Namespace(`${baseName}-bus`, {
        resourceGroupName: resourceGroup.name,
        sku: "Standard",
    });

    const activityCommandsQueue = new azure.servicebus.Queue('activitycommands', {
        namespaceName: serviceBusNamespace.name,
        resourceGroupName: resourceGroup.name,
        name: 'activitycommands'
    });

    const orderCommandsQueue = new azure.servicebus.Queue('ordercommands', {
        namespaceName: serviceBusNamespace.name,
        resourceGroupName: resourceGroup.name,
        name: 'ordercommands'
    });

    const activityEventsTopic = new azure.servicebus.Topic('activityevents', {
        namespaceName: serviceBusNamespace.name,
        resourceGroupName: resourceGroup.name,
        name: 'activityevents'
    });

    const orderEventsTopic = new azure.servicebus.Topic('orderevents', {
        namespaceName: serviceBusNamespace.name,
        resourceGroupName: resourceGroup.name,
        name: 'orderevents'
    });

    //
    // Redis Cache
    //

    const redisCache = new azure.redis.Cache('cmds', {
        resourceGroupName: resourceGroup.name,
        capacity: 0,
        family: 'C',
        skuName: 'Basic'
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
    const eventHubNamespace = new azure.eventhub.EventHubNamespace('tixhub', {
        resourceGroupName: resourceGroup.name,
        sku: "Standard",
        capacity: 1,
    });

    const activityEventHub = new azure.eventhub.EventHub('activity', {
        resourceGroupName: resourceGroup.name,
        namespaceName: eventHubNamespace.name,
        partitionCount: 32,
        messageRetention: 1,
    });

    const activityViewsConsumerGroup = new azure.eventhub.ConsumerGroup('activityviews', {
        resourceGroupName: resourceGroup.name,
        namespaceName: eventHubNamespace.name,
        eventhubName: activityEventHub.name,
    });

    const activityProcessConsumerGroup = new azure.eventhub.ConsumerGroup('activityproc', {
        resourceGroupName: resourceGroup.name,
        namespaceName: eventHubNamespace.name,
        eventhubName: activityEventHub.name,
    });

    const orderEventHub = new azure.eventhub.EventHub('orders', {
        resourceGroupName: resourceGroup.name,
        namespaceName: eventHubNamespace.name,
        partitionCount: 32,
        messageRetention: 1,
    });

    const orderViewsConsumerGroup = new azure.eventhub.ConsumerGroup('orderviews', {
        resourceGroupName: resourceGroup.name,
        namespaceName: eventHubNamespace.name,
        eventhubName: orderEventHub.name,
    });

    const orderProcessConsumerGroup = new azure.eventhub.ConsumerGroup('orderproc', {
        resourceGroupName: resourceGroup.name,
        namespaceName: eventHubNamespace.name,
        eventhubName: orderEventHub.name,
    });

    //
    // App Insights
    //
    const appInsights = new azure.appinsights.Insights(`${baseName}-ai`, {
        resourceGroupName: resourceGroup.name,

        applicationType: "web",
    });


    // Collect settings
    const activityEventStoreConnection = pulumi.all([sqlServer.name, activityEventStoreDatabase.name, sqlServer.administratorLoginPassword]).apply(([server, db, pwd]) =>
        `Server=tcp:${server}.database.windows.net;initial catalog=${db};user ID=${sqlAdminUser};password=${pwd};Persist Security Info=False;Encrypt=True;MultipleActiveResultSets=False;Connection Timeout=30;`);
    const orderEventStoreConnection = pulumi.all([sqlServer.name, orderEventStoreDatabase.name, sqlServer.administratorLoginPassword]).apply(([server, db, pwd]) =>
        `Server=tcp:${server}.database.windows.net;initial catalog=${db};user ID=${sqlAdminUser};password=${pwd};Persist Security Info=False;Encrypt=True;MultipleActiveResultSets=False;Connection Timeout=30;`);
    const processEventStoreConnection = pulumi.all([sqlServer.name, processEventStoreDatabase.name, sqlServer.administratorLoginPassword]).apply(([server, db, pwd]) =>
        `Server=tcp:${server}.database.windows.net;initial catalog=${db};user ID=${sqlAdminUser};password=${pwd};Persist Security Info=False;Encrypt=True;MultipleActiveResultSets=False;Connection Timeout=30;`);
    const tixViewsConnection = pulumi.all([sqlServer.name, tixViewsDatabase.name, sqlServer.administratorLoginPassword]).apply(([server, db, pwd]) =>
        `Server=tcp:${server}.database.windows.net;initial catalog=${db};user ID=${sqlAdminUser};password=${pwd};Persist Security Info=False;Encrypt=True;MultipleActiveResultSets=False;Connection Timeout=30;`);

    const activityAppSettings = {
        runtime: "dotnet",
        TixServiceBus: serviceBusNamespace.defaultPrimaryConnectionString,
        ActivityCommandsQueue: activityCommandsQueue.name,
        ActivityEventPublisherTopic: activityEventsTopic.name,
        "APPINSIGHTS_INSTRUMENTATIONKEY": appInsights.instrumentationKey,
        ActivityEventsDbConnection: activityEventStoreConnection,
        ViewsDbConnection: tixViewsConnection,
        AzureWebJobsStorage: storageAccount.primaryConnectionString,
        EventHubConnectionString: eventHubNamespace.defaultPrimaryConnectionString,
        ActivityEventHubName: activityEventHub.name,
        ActivityViewsConsumerGroup: activityViewsConsumerGroup.name,
        CommandLogCache: redisCache.primaryConnectionString,
    };
    const orderAppSettings = {
        runtime: "dotnet",
        TixServiceBus: serviceBusNamespace.defaultPrimaryConnectionString,
        OrderCommandsQueue: orderCommandsQueue.name,
        OrderEventPublisherTopic: orderEventsTopic.name,
        "APPINSIGHTS_INSTRUMENTATIONKEY": appInsights.instrumentationKey,
        OrderEventsDbConnection: orderEventStoreConnection,
        ViewsDbConnection: tixViewsConnection,
        AzureWebJobsStorage: storageAccount.primaryConnectionString,
        EventHubConnectionString: eventHubNamespace.defaultPrimaryConnectionString,
        OrderEventHubName: orderEventHub.name,
        OrderViewsConsumerGroup: orderViewsConsumerGroup.name,
        CommandLogCache: redisCache.primaryConnectionString,
    };
    const processAppSettings = {
        runtime: "dotnet",
        TixServiceBus: serviceBusNamespace.defaultPrimaryConnectionString,
        ActivityCommandsQueue: activityCommandsQueue.name,
        OrderCommandsQueue: orderCommandsQueue.name,
        "APPINSIGHTS_INSTRUMENTATIONKEY": appInsights.instrumentationKey,
        AzureWebJobsStorage: storageAccount.primaryConnectionString,
        ProcessEventsDbConnection: processEventStoreConnection,
        EventHubConnectionString: eventHubNamespace.defaultPrimaryConnectionString,
        ActivityEventHubName: activityEventHub.name,
        ActivityProcessConsumerGroup: activityProcessConsumerGroup.name,
        OrderEventHubName: orderEventHub.name,
        OrderProcessConsumerGroup: orderProcessConsumerGroup.name
    };
    const apiAppSettings = {
        runtime: "dotnet",
        "APPINSIGHTS_INSTRUMENTATIONKEY": appInsights.instrumentationKey,
        AzureWebJobsStorage: storageAccount.primaryConnectionString,
        TixServiceBus: serviceBusNamespace.defaultPrimaryConnectionString,
        ActivityCommandsQueue: activityCommandsQueue.name,
        OrderCommandsQueue: orderCommandsQueue.name,
        ViewsDbConnection: tixViewsConnection,
        //SignalRConnection: signalR.primaryConnectionString,
        //SignalRHost: signalR.hostname,
        CommandLogCache: redisCache.primaryConnectionString,
    };

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
        const activityApp = new azure.appservice.ArchiveFunctionApp(`${baseName}-activity`, {
            resourceGroupName: resourceGroup.name,
            archive: new pulumi.asset.FileArchive("../dist/Swetugg.Tix.Activity.Funcs.zip"),
            account: storageAccount,
            version: '~3',
            appSettings: activityAppSettings,
        });

        const orderApp = new azure.appservice.ArchiveFunctionApp(`${baseName}-order`, {
            resourceGroupName: resourceGroup.name,
            archive: new pulumi.asset.FileArchive("../dist/Swetugg.Tix.Order.Funcs.zip"),
            account: storageAccount,
            version: '~3',
            appSettings: orderAppSettings,
        });

        const processApp = new azure.appservice.ArchiveFunctionApp(`${baseName}-process`, {
            resourceGroupName: resourceGroup.name,
            archive: new pulumi.asset.FileArchive("../dist/Swetugg.Tix.Process.Funcs.zip"),
            account: storageAccount,
            version: '~3',
            appSettings: processAppSettings,
        });

        const apiApp = new azure.appservice.ArchiveFunctionApp(`${baseName}-api`, {
            resourceGroupName: resourceGroup.name,
            archive: new pulumi.asset.FileArchive("../dist/Swetugg.Tix.Api.zip"),
            account: storageAccount,
            version: '~3',
            appSettings: apiAppSettings,
            siteConfig: {
                cors: { allowedOrigins: ["*"] }
            },
        });

        apiHostname = apiApp.functionApp.defaultHostname;

        //
        // Frontend Apps
        //

        // We need a separate resource group if we want to use a Linux AppService Plan
        const frontendResourceGroup = new azure.core.ResourceGroup(`${baseName}-fe-group`, { location: mainLocation });
        frontendResourceGroupName = frontendResourceGroup.name;

        const frontendServicePlan = new azure.appservice.Plan("fe-plan", {
            resourceGroupName: frontendResourceGroup.name,
            sku: {
                tier: 'Basic',
                size: 'B1'
            }/*,
            kind: azure.appservice.Kinds.Linux,
            reserved: true*/
        });

        /*
        const frontendStorageAccount = new azure.storage.Account("festorage", {
            // The location for the storage account will be derived automatically from the resource group.
            resourceGroupName: frontendResourceGroup.name,
            accountTier: "Standard",
            accountReplicationType: "LRS",
        });

        const appServiceStorageContainer = new azure.storage.Container(`${baseName}-code`, {
            storageAccountName: frontendStorageAccount.name,
            containerAccessType: "private",
        });

        const frontpageBlob = new azure.storage.Blob(`${baseName}-fb`, {
            storageAccountName: frontendStorageAccount.name,
            storageContainerName: appServiceStorageContainer.name,
            
            type: "Block",
        
            source: new pulumi.asset.FileArchive("../dist/frontpage.zip"),
        });

        const backOfficeBlob = new azure.storage.Blob(`${baseName}-bb`, {
            storageAccountName: frontendStorageAccount.name,
            storageContainerName: appServiceStorageContainer.name,
            type: "Block",
            source: new pulumi.asset.FileArchive("../dist/back-office.zip"),
        });

        const frontpageCodeBlobUrl = azure.storage.signedBlobReadUrl(frontpageBlob, frontendStorageAccount);
        const backOfficeCodeBlobUrl = azure.storage.signedBlobReadUrl(backOfficeBlob, frontendStorageAccount);
        */
        const frontpageApp = new azure.appservice.AppService(`${baseName}-frontpage`, {
            resourceGroupName: frontendResourceGroup.name,
            appServicePlanId: frontendServicePlan.id,
            appSettings: {
                APPINSIGHTS_INSTRUMENTATIONKEY: appInsights.instrumentationKey,
                APPLICATIONINSIGHTS_CONNECTION_STRING: pulumi.interpolate `InstrumentationKey=${appInsights.instrumentationKey}`,
                ApplicationInsightsAgent_EXTENSION_VERSION: "~2",
                SCM_DO_BUILD_DURING_DEPLOYMENT: "true",
                WEBSITE_NODE_DEFAULT_VERSION: "12.18.0",
                NEXT_PUBLIC_API_ROOT: pulumi.interpolate `https://${apiHostname}/api`
            },
        });

        const backOfficeApp = new azure.appservice.AppService(`${baseName}-backoffice`, {
            resourceGroupName: frontendResourceGroup.name,
            appServicePlanId: frontendServicePlan.id,
            appSettings: {
                APPINSIGHTS_INSTRUMENTATIONKEY: appInsights.instrumentationKey,
                APPLICATIONINSIGHTS_CONNECTION_STRING: pulumi.interpolate `InstrumentationKey=${appInsights.instrumentationKey}`,
                ApplicationInsightsAgent_EXTENSION_VERSION: "~2",                
                SCM_DO_BUILD_DURING_DEPLOYMENT: "true",
                WEBSITE_NODE_DEFAULT_VERSION: "12.18.0",
                NEXT_PUBLIC_API_ROOT: pulumi.interpolate `https://${apiHostname}/api`
            },
        });

        backOfficeHostname = backOfficeApp.defaultSiteHostname;
        frontpageHostname = frontpageApp.defaultSiteHostname;
        backOfficeAppName = backOfficeApp.name;
        frontpageAppName = frontpageApp.name;
    }

    const output = {
        // Return some connection strings
        storageConnectionString: storageAccount.primaryConnectionString,
        serviceBusConnectionString: serviceBusNamespace.defaultPrimaryConnectionString,
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

