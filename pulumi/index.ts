import * as pulumi from "@pulumi/pulumi";
import * as azure from "@pulumi/azure";
import * as random from "@pulumi/random";
import { getPublicIp } from "./local-ip";

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

    // Generate a random password
    const sqlAdminPassword = new random.RandomPassword("tixdbpassword", {
        length: 16,
        special: false
    });

    const sqlServer = new azure.sql.SqlServer("tixdb", {
        resourceGroupName: resourceGroup.name,
        version: "12.0",
        administratorLogin: sqlAdminUser,
        administratorLoginPassword: sqlAdminPassword.result
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

    const ticketEventStoreDatabase = new azure.sql.Database("ticketevents", {
        resourceGroupName: resourceGroup.name,
        serverName: sqlServer.name,
        requestedServiceObjectiveName: "S0",
    });

    const tixViewsDatabase = new azure.sql.Database("tixviews", {
        resourceGroupName: resourceGroup.name,
        serverName: sqlServer.name,
        requestedServiceObjectiveName: "S0",
    });

    const serviceBusNamespace = new azure.servicebus.Namespace(`${baseName}-bus`, {
        resourceGroupName: resourceGroup.name,
        sku: "Standard",
    });

    const activityCommandsQueue = new azure.servicebus.Queue('activitycommands', {
        namespaceName: serviceBusNamespace.name,
        resourceGroupName: resourceGroup.name,
        name: 'activitycommands'
    });

    const ticketCommandsQueue = new azure.servicebus.Queue('ticketcommands', {
        namespaceName: serviceBusNamespace.name,
        resourceGroupName: resourceGroup.name,
        name: 'ticketcommands'
    });

    const activityEventsTopic = new azure.servicebus.Topic('activityevents', {
        namespaceName: serviceBusNamespace.name,
        resourceGroupName: resourceGroup.name,
        name: 'activityevents'
    });

    const ticketEventsTopic = new azure.servicebus.Topic('ticketevents', {
        namespaceName: serviceBusNamespace.name,
        resourceGroupName: resourceGroup.name,
        name: 'ticketevents'
    });

    const processActivitySubscription = new azure.servicebus.Subscription('processactsub', {
        namespaceName: serviceBusNamespace.name,
        topicName: activityEventsTopic.name,
        resourceGroupName: resourceGroup.name,
        maxDeliveryCount: 10,
        name: 'processactsub'
    });

    const appInsights = new azure.appinsights.Insights(`${baseName}-ai`, {
        resourceGroupName: resourceGroup.name,

        applicationType: "web",
    });


    // Collect settings
    const activityEventStoreConnection = pulumi.all([sqlServer.name, activityEventStoreDatabase.name, sqlServer.administratorLoginPassword]).apply(([server, db, pwd]) =>
        `Server=tcp:${server}.database.windows.net;initial catalog=${db};user ID=${sqlAdminUser};password=${pwd};Persist Security Info=False;Encrypt=True;MultipleActiveResultSets=False;Connection Timeout=30;`);
    const ticketEventStoreConnection = pulumi.all([sqlServer.name, ticketEventStoreDatabase.name, sqlServer.administratorLoginPassword]).apply(([server, db, pwd]) =>
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
        AzureWebJobsStorage: storageAccount.primaryConnectionString
    };
    const ticketAppSettings = {
        runtime: "dotnet",
        TixServiceBus: serviceBusNamespace.defaultPrimaryConnectionString,
        TicketCommandsQueue: ticketCommandsQueue.name,
        TicketEventPublisherTopic: ticketEventsTopic.name,
        "APPINSIGHTS_INSTRUMENTATIONKEY": appInsights.instrumentationKey,
        TicketEventsDbConnection: ticketEventStoreConnection,
        ViewsDbConnection: tixViewsConnection,
        AzureWebJobsStorage: storageAccount.primaryConnectionString
    };
    const processAppSettings = {
        runtime: "dotnet",
        TixServiceBus: serviceBusNamespace.defaultPrimaryConnectionString,
        ActivityEventPublisherTopic: activityEventsTopic.name,
        ProcessActivitySub: processActivitySubscription.name,
        "APPINSIGHTS_INSTRUMENTATIONKEY": appInsights.instrumentationKey,
        AzureWebJobsStorage: storageAccount.primaryConnectionString
    };
    const apiAppSettings = {
        runtime: "dotnet",
        "APPINSIGHTS_INSTRUMENTATIONKEY": appInsights.instrumentationKey,
        AzureWebJobsStorage: storageAccount.primaryConnectionString,
        TixServiceBus: serviceBusNamespace.defaultPrimaryConnectionString,
        ActivityCommandsQueue: activityCommandsQueue.name,
        ViewsDbConnection: tixViewsConnection,
    };

    let hostname: pulumi.Output<string> | undefined;

    if (shouldDeploy) {

        const activityApp = new azure.appservice.ArchiveFunctionApp(`${baseName}-activity`, {
            resourceGroupName: resourceGroup.name,
            archive: new pulumi.asset.FileArchive("../dist/Swetugg.Tix.Activity.Funcs.zip"),
            account: storageAccount,
            version: '~3',
            appSettings: activityAppSettings,
        });

        const ticketApp = new azure.appservice.ArchiveFunctionApp(`${baseName}-ticket`, {
            resourceGroupName: resourceGroup.name,
            archive: new pulumi.asset.FileArchive("../dist/Swetugg.Tix.Ticket.Funcs.zip"),
            account: storageAccount,
            version: '~3',
            appSettings: ticketAppSettings
        });

        const processApp = new azure.appservice.ArchiveFunctionApp(`${baseName}-process`, {
            resourceGroupName: resourceGroup.name,
            archive: new pulumi.asset.FileArchive("../dist/Swetugg.Tix.Process.Funcs.zip"),
            account: storageAccount,
            version: '~3',
            appSettings: processAppSettings
        });

        const apiApp = new azure.appservice.ArchiveFunctionApp(`${baseName}-api`, {
            resourceGroupName: resourceGroup.name,
            archive: new pulumi.asset.FileArchive("../dist/Swetugg.Tix.Api.zip"),
            account: storageAccount,
            version: '~3',
            appSettings: apiAppSettings,
            siteConfig: {
                cors: { allowedOrigins : ["*"] }
            }
        });

        hostname = apiApp.functionApp.defaultHostname;
    }

    const output = {
        // Return some connection strings
        storageConnectionString: storageAccount.primaryConnectionString,
        serviceBusConnectionString: serviceBusNamespace.defaultPrimaryConnectionString,
        activityEventStoreConnectionString: activityEventStoreConnection,
        ticketEventStoreConnectionString: ticketEventStoreConnection,
        tixViewsConnectionString: tixViewsConnection,
        apiHostName: hostname
    };

    return { 
        out: output, 
        activityAppSettings: {
            IsEncrypted: false,
            Values: activityAppSettings
        },
        ticketAppSettings: {
            IsEncrypted: false,
            Values: ticketAppSettings
        },
        processAppSettings: {
            IsEncrypted: false,
            Values: processAppSettings
        },
        apiAppSettings: {
            IsEncrypted: false,
            Values: apiAppSettings,
            Host: {
                CORS: "*"
            }
        }
     };
}

