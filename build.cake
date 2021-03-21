#addin "Cake.MsDeploy"
#addin "Cake.Yarn"

// Target - The task you want to start. Runs the Default task if not specified.
var target = Argument("Target", "Default");  
var configuration = Argument("Configuration", "Release");

Information($"Running target {target} in configuration {configuration}");

var distDirectory = Directory("./dist");

// Deletes the contents of the Artifacts folder if it contains anything from a previous build.
Task("Clean")  
    .Does(() =>
    {
        CleanDirectory(distDirectory);
    });

// Run dotnet restore to restore all package references.
Task("Restore")  
    .Does(() =>
    {
        DotNetCoreRestore();
    });

// Build using the build configuration specified as an argument.
 Task("Build")
    .Does(() =>
    {
        DotNetCoreBuild(".",
            new DotNetCoreBuildSettings()
            {
                Configuration = configuration,
                ArgumentCustomization = args => args.Append("--no-restore"),
            });
    });

// Look under a 'Tests' folder and run dotnet test against all of those projects.
// Then drop the XML test results file in the Artifacts folder at the root.
Task("Test")  
    .Does(() =>
    {
        var projects = GetFiles("./test/**/*.csproj") - GetFiles("./test/**/*Helpers.csproj");
        foreach(var project in projects)
        {
            Information("Testing project " + project);
            DotNetCoreTest(
                project.ToString(),
                new DotNetCoreTestSettings()
                {
                    Configuration = configuration,
                    NoBuild = true,
                    ArgumentCustomization = args => args.Append("--no-restore"),
                });
        }
    });

Task("BackOffice-Install")
    .Does(() => 
    {
        StartProcess("yarn.cmd", new ProcessSettings() {
           WorkingDirectory = "./back-office",
        });
    });

Task("BackOffice-Build")
    .IsDependentOn("BackOffice-Install")
    .Does(() => 
    {
        StartProcess("yarn.cmd", new ProcessSettings() {
           Arguments = "build",
           WorkingDirectory = "./back-office" 
        });
    });

Task("BackOffice-Package")
    .IsDependentOn("BackOffice-Build")
    .Does(() => 
    {
        CreateFrontendPackage("back-office");
    });


Task("Frontpage-Install")
    .Does(() => 
    {
        StartProcess("yarn.cmd", new ProcessSettings() {
           WorkingDirectory = "./back-office",
        });
    });

Task("Frontpage-Build")
    .IsDependentOn("Frontpage-Install")
    .Does(() => 
    {
        StartProcess("yarn.cmd", new ProcessSettings() {
           Arguments = "build",
           WorkingDirectory = "./back-office" 
        });
    });

Task("Frontpage-Package")
    .IsDependentOn("Frontpage-Build")
    .Does(() => 
    {
        CreateFrontendPackage("frontpage");
    });

public void CreateFrontendPackage(string folderName) 
{
        var backOfficeFolder = Directory($"./{folderName}");
        var filesToZip = GetFiles(backOfficeFolder.Path.FullPath + "/**/*", new GlobberSettings() {
            Predicate = (dir) => {
                return !(dir.Path.FullPath.Contains("node_modules") || dir.Path.FullPath.Contains(".next"));
            }
        });

        var zipFile = distDirectory.Path.GetFilePath($"{folderName}.zip");
        Zip(backOfficeFolder, zipFile, filesToZip);
}
public void CreateDotNetPackage(string projectName)
{
    var distFolder = distDirectory.Path.Combine(projectName);
    var zipFile = distDirectory.Path.GetFilePath($"{projectName}.zip");
    var projectFile = $"./src/{projectName}/{projectName}.csproj"; 
    
    DotNetCorePublish(
        projectFile,
        new DotNetCorePublishSettings()
        {
            Configuration = configuration,
            OutputDirectory = distFolder,
            ArgumentCustomization = args => args.Append("--no-restore"),
        });

    Zip(distFolder, zipFile);
}

// Publish the app to the /dist folder
Task("Package")  
    .Does(() =>
    {
        CreateDotNetPackage("Swetugg.Tix.Activity.Funcs");
        CreateDotNetPackage("Swetugg.Tix.Order.Funcs");
        CreateDotNetPackage("Swetugg.Tix.Process.Funcs");
        CreateDotNetPackage("Swetugg.Tix.Api");
    });

// A meta-task that runs all the steps to Build and Test the app
Task("BuildAndTest")  
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .IsDependentOn("Build")
    .IsDependentOn("Test");

// The default task to run if none is explicitly specified. In this case, we want
// to run everything starting from Clean, all the way up to Publish.
Task("Default")  
    .IsDependentOn("BuildAndTest")
    .IsDependentOn("Package")
    .IsDependentOn("Frontpage-Package")
    .IsDependentOn("BackOffice-Package");

// Executes the task specified in the target argument.
RunTarget(target);
