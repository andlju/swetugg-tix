import * as pulumi from "@pulumi/pulumi";
import * as azure from "@pulumi/azure";

const mainLocation = azure.Locations.NorthEurope;

const env = "dev";
const baseName = `tix-${env}`;

// Create an Azure Resource Group
const resourceGroup = new azure.core.ResourceGroup(`${baseName}-group`, { location : mainLocation });

// Create an Azure resource (Storage Account)
const storageAccount = new azure.storage.Account("storage", {
    // The location for the storage account will be derived automatically from the resource group.
    resourceGroupName: resourceGroup.name,
    accountTier: "Standard",
    accountReplicationType: "LRS",
});

const serviceBusNamespace = new azure.servicebus.Namespace(`${baseName}-bus`, {
    resourceGroupName: resourceGroup.name,
    sku: "Standard",
});

const activityCommandsQueue = new azure.servicebus.Queue('activitycommands', {
    namespaceName: serviceBusNamespace.name,
    resourceGroupName: resourceGroup.name,
});

const ticketCommandsQueue = new azure.servicebus.Queue('ticketcommands', {
    namespaceName: serviceBusNamespace.name,
    resourceGroupName: resourceGroup.name,
});

const activityEventsTopic = new azure.servicebus.Topic('activityevents', {
    namespaceName: serviceBusNamespace.name,
    resourceGroupName: resourceGroup.name,
});

const ticketEventsTopic = new azure.servicebus.Topic('ticketevents', {
    namespaceName: serviceBusNamespace.name,
    resourceGroupName: resourceGroup.name,
});

const processActivitySubscription = new azure.servicebus.Subscription('processactsub', {
    namespaceName: serviceBusNamespace.name,
    topicName: activityEventsTopic.name,
    resourceGroupName: resourceGroup.name,
    maxDeliveryCount: 10
});

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
        "APPINSIGHTS_INSTRUMENTATIONKEY": appInsights.instrumentationKey
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
        "APPINSIGHTS_INSTRUMENTATIONKEY": appInsights.instrumentationKey
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

// Return the host name of the api
export const apiHostName = apiApp.functionApp.defaultHostname;
