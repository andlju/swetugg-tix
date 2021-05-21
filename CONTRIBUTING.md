Getting started
===============

Prerequisites
-------------
For a smoother experience, make sure you have [Chocolatey](https://chocolatey.org/) installed.

You will need the following tools to build and run Swetugg Tix:

 * Pulumi (`choco install pulumi`)
 * An Azure account
 * Azure CLI (`choco install azure-cli`)
 * Azure Functions Core tools 3 (`choco install azure-functions-core-tools-3`)
 * Node JS (`choco install nodejs`)
 * Yarn (`npm install -g yarn`)
 * Cake tool (`dotnet tool install --global Cake.Tool`)
 * Visual Studio Code and/or Visual Studio

Building Swetugg Tix
--------------------
To build the project locally, run the command:

```
dotnet cake
```

This will build all .NET assets, run all unit tests and create packages that are ready to be pushed to Azure.

Pulumi
------
Swetugg Tix uses Pulumi to set up an Azure Resource Group with all the necessary databases, queues and storage needed for it to run. There are currently two Pulumi "stacks". 

The `local` stack will only contain databases, queues etc. It can be used when debugging Swetugg Tix locally.

The `dev` stack is complete with all Azure Functions. To run it you first need to [build the project](#building-swetugg-tix).

Using the Local stack
---------------------
In order to run the project locally you'll first want to use Pulumi to set up the `local` stack. The easiest way to do this is to run the `Prepare-LocalEnvironment.ps1` command found in the root of the repository. This will first make sure to create the `local` Pulumi stack in Azure, then get the output settings from the stack and copy them as `local.settings.json` files for each project.

Client
------
First, make sure that yarn has installed all dependencies:

```
cd .\client
yarn
```

Then you can run the client with `yarn dev`

Azure AD B2C
-------------------
Swetugg Tix relies on Azure AD B2C for authentication, unfortunately at this time there is no way to automate the setup of an AD B2C tenant so these steps have to be completed manually. Values from this setup need to be stored as secrets in Pulumi before setting up the environment. To facilitate that, create a file in the `./pulumi` folder called `.azure-ad.secret.json`. You can use the `.azure-ad.secret.json.example` file as a reference.

### Enable resource provider

Make sure that the `Microsoft.AzureActiveDirectory` is enabled on your Subscription.
To enable it you can call `az provider register --namespace Microsoft.AzureActiveDirectory`

### AD B2C

Create a new AD B2C Tenant by following these instructions:
https://docs.microsoft.com/en-us/azure/active-directory-b2c/tutorial-create-tenant

Set the tenantName and tenantId properties in `.azure-ad.secret.json`

### Register the Backoffice SPA app

In the new tenant, register a new application (https://docs.microsoft.com/en-us/azure/active-directory-b2c/tutorial-register-applications?tabs=app-reg-ga)

Set the Redirect URI to Single Page Application. For local development, the URL should be http://localhost:3000

Set the backofficeApp property in `.azure-ad.secret.json` to the Application (client) ID of the newly registered app.

### Register the API app

Register another app for the API. This time as a Web app.

Create a new Client Secret.

Set the apiApp property to the Application ID of the new app, and the apiAppSecret property to the value of the secret you created.

### Create a Scope

In the API application, select "Expose an API".

1) set the App ID Uri to something like `{base url}/tix-api`
2) Create a scope `access_as_backoffice`

In the Backoffice application, select "API Permissions"

1) Select "Add a permission"
2) Under My APIs, select the API application and check the `access_as_backoffice` permission
3) Add the permission
4) Grant admin consent

Set the backofficeScopeName property to `access_as_backoffice`.

### Create a User flow

Create a new Sign up and Sign in User Flow.

Select the following attributes:
* Display Name (Collect & Return)
* User is new (Return)
* UserId (Return)

Set the policyName property to the name of the newly created sign in / sign up flow.

### Create the Pulumi Secrets

Run the Powershell script `.\Setup-Secrets.ps1` to create the necessary Pulumi Secrets.