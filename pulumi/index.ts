import * as pulumi from "@pulumi/pulumi";
import * as azure from "@pulumi/azure";
import * as random from "@pulumi/random";

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

const sqlServer = new azure.sql.SqlServer("tixdb", {
    resourceGroupName: resourceGroup.name,
    version: "12.0",
    administratorLogin: sqlAdminUser,
    administratorLoginPassword: sqlAdminPassword.result
});

const eventStoreDatabase = new azure.sql.Database("tixevent", {
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

const eventStoreConnection = pulumi.all([sqlServer.name, eventStoreDatabase.name, sqlServer.administratorLoginPassword]).apply(([server, db, pwd]) =>
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
            TixDbConnection: eventStoreConnection
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
            TixDbConnection: eventStoreConnection
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
}

// Return some connection strings
export const storageConnectionString = storageAccount.primaryConnectionString;
export const serviceBusConnectionString = serviceBusNamespace.defaultPrimaryConnectionString;
export const eventStoreConnectionString = eventStoreConnection;

// Return the host name of the api (if deployed)
export const apiHostName = hostname;

