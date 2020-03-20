import * as pulumi from "@pulumi/pulumi";
import * as azure from "@pulumi/azure";
import * as random from "@pulumi/random";
import { SqlContainer } from "@pulumi/azure/cosmosdb";

const mainLocation = azure.Locations.NorthEurope;

const config = new pulumi.Config();
const shouldDeploy = config.requireBoolean("deploy");

const stack = pulumi.getStack();
const baseName = `tix-${stack}`;
const sqlAdminUser = "tixadmin";

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
    special: true
});

const viewsCosmosAccount = new azure.cosmosdb.Account("tixviews", {
    resourceGroupName: resourceGroup.name,
    consistencyPolicy: {
        consistencyLevel: "Eventual"
    },
    offerType: "Standard",
    geoLocations: [{ location: resourceGroup.location, failoverPriority: 0 }]
});

const viewsCosmosDb = new azure.cosmosdb.SqlDatabase("tixviews", {
    resourceGroupName: resourceGroup.name,
    accountName: viewsCosmosAccount.name
});

const viewsContainer = new azure.cosmosdb.SqlContainer("tixviews", {
    resourceGroupName: resourceGroup.name,
    databaseName: viewsCosmosDb.name,
    accountName: viewsCosmosAccount.name,
    partitionKeyPath: "/ActivityId",
});

const sqlServer = new azure.sql.SqlServer("tixdb", {
    resourceGroupName: resourceGroup.name,
    version: "12.0",
    administratorLogin: sqlAdminUser,
    administratorLoginPassword: sqlAdminPassword.result,
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

let hostname: pulumi.Output<string> | undefined;

const activityEventStoreConnection = pulumi.all([sqlServer.name, activityEventStoreDatabase.name, sqlServer.administratorLoginPassword]).apply(([server, db, pwd]) =>
    `Server=tcp:${server}.database.windows.net;initial catalog=${db};user ID=${sqlAdminUser};password=${pwd};Min Pool Size=0;Max Pool Size=30;Persist Security Info=true;`);
const ticketEventStoreConnection = pulumi.all([sqlServer.name, ticketEventStoreDatabase.name, sqlServer.administratorLoginPassword]).apply(([server, db, pwd]) =>
    `Server=tcp:${server}.database.windows.net;initial catalog=${db};user ID=${sqlAdminUser};password=${pwd};Min Pool Size=0;Max Pool Size=30;Persist Security Info=true;`);
const tixViewsConnection = pulumi.all([sqlServer.name, tixViewsDatabase.name, sqlServer.administratorLoginPassword]).apply(([server, db, pwd]) =>
    `Server=tcp:${server}.database.windows.net;initial catalog=${db};user ID=${sqlAdminUser};password=${pwd};Min Pool Size=0;Max Pool Size=30;Persist Security Info=true;`);

if (shouldDeploy) {
    const appInsights = new azure.appinsights.Insights(`${baseName}-ai`, {
        resourceGroupName: resourceGroup.name,

        applicationType: "web",
    });

    const activityApp = new azure.appservice.ArchiveFunctionApp(`${baseName}-activity`, {
        resourceGroupName: resourceGroup.name,
        archive: new pulumi.asset.FileArchive("../dist/Swetugg.Tix.Activity.Funcs.zip"),
        account: storageAccount,
        version: '~3',
        appSettings: {
            runtime: "dotnet",
            TixServiceBus: serviceBusNamespace.defaultPrimaryConnectionString,
            ActivityCommandsQueue: activityCommandsQueue.name,
            EventPublisherTopic: activityEventsTopic.name,
            "APPINSIGHTS_INSTRUMENTATIONKEY": appInsights.instrumentationKey,
            ActivityEventsDbConnection: activityEventStoreConnection,
            ViewsDbConnection: tixViewsConnection,
            ViewsEndpointUrl: viewsCosmosAccount.endpoint,
            ViewsAuthorizationKey: viewsCosmosAccount.primaryMasterKey,
            ViewsDatabaseName: viewsCosmosDb.name,
            ViewsContainerName: viewsContainer.name
        },
    });

    const ticketApp = new azure.appservice.ArchiveFunctionApp(`${baseName}-ticket`, {
        resourceGroupName: resourceGroup.name,
        archive: new pulumi.asset.FileArchive("../dist/Swetugg.Tix.Ticket.Funcs.zip"),
        account: storageAccount,
        version: '~3',
        appSettings: {
            runtime: "dotnet",
            TixServiceBus: serviceBusNamespace.defaultPrimaryConnectionString,
            TicketCommandsQueue: ticketCommandsQueue.name,
            EventPublisherTopic: ticketEventsTopic.name,
            "APPINSIGHTS_INSTRUMENTATIONKEY": appInsights.instrumentationKey,
            TicketEventsDbConnection: ticketEventStoreConnection,
            ViewsDbConnection: tixViewsConnection
        },
    });

    const processApp = new azure.appservice.ArchiveFunctionApp(`${baseName}-process`, {
        resourceGroupName: resourceGroup.name,
        archive: new pulumi.asset.FileArchive("../dist/Swetugg.Tix.Process.Funcs.zip"),
        account: storageAccount,
        version: '~3',
        appSettings: {
            runtime: "dotnet",
            TixServiceBus: serviceBusNamespace.defaultPrimaryConnectionString,
            ActivityEventsTopic: activityEventsTopic.name,
            ProcessActivitySub: processActivitySubscription.name,
            "APPINSIGHTS_INSTRUMENTATIONKEY": appInsights.instrumentationKey
        },
    });

    const apiApp = new azure.appservice.ArchiveFunctionApp(`${baseName}-api`, {
        resourceGroupName: resourceGroup.name,
        archive: new pulumi.asset.FileArchive("../dist/Swetugg.Tix.Api.zip"),
        account: storageAccount,
        version: '~3',
        appSettings: {
            runtime: "dotnet",
            TixServiceBus: serviceBusNamespace.defaultPrimaryConnectionString,
            ActivityCommandsQueue: activityCommandsQueue.name,
            "APPINSIGHTS_INSTRUMENTATIONKEY": appInsights.instrumentationKey
        },
    });
    hostname = apiApp.functionApp.defaultHostname;

    const allApps = [activityApp, ticketApp, processApp, apiApp];

    for (const app of allApps) {
        app.functionApp.name.apply(appName => {
            const firewallRules = app.functionApp.outboundIpAddresses.apply(
                ips => ips.split(',').map(
                    (ip,idx) => new azure.sql.FirewallRule(`FR_${appName}_${idx}`, {
                        startIpAddress: ip,
                        endIpAddress: ip,
                        resourceGroupName: resourceGroup.name,
                        serverName: sqlServer.name,
                        name: `FR_${appName}_${idx}`
                    })
                ));
            });
    }
}

// Return some connection strings
export const storageConnectionString = storageAccount.primaryConnectionString;
export const serviceBusConnectionString = serviceBusNamespace.defaultPrimaryConnectionString;
export const activityEventStoreConnectionString = activityEventStoreConnection;
export const ticketEventStoreConnectionString = ticketEventStoreConnection;
export const tixViewsConnectionString = tixViewsConnection;

export const viewsEndpointUrl = viewsCosmosAccount.endpoint;
export const viewsAuthorizationKey = viewsCosmosAccount.primaryMasterKey;
export const viewsDatabaseName = viewsCosmosDb.name;
export const viewsContainerName = viewsContainer.name;

// Return the host name of the api (if deployed)
export const apiHostName = hostname;

