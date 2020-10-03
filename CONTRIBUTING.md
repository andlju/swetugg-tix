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
