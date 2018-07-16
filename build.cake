#addin "Cake.MsDeploy"

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

public void CreatePackage(string projectName)
{
    var distFolder = distDirectory.Path.Combine(projectName);
    var zipFile = distDirectory.Path.Combine($"{projectName}.zip");
    var projectFile = $"./src/{projectName}/{projectName}.csproj"; 
    
    DotNetCorePublish(
        projectFile,
        new DotNetCorePublishSettings()
        {
            Configuration = configuration,
            OutputDirectory = distFolder,
            ArgumentCustomization = args => args.Append("--no-restore"),
        });

    MsDeploy(new MsDeploySettings() {
        Source = new DirPathProvider() {
            Path = MakeAbsolute(distFolder).ToString()
        },
        Destination = new PackageProvider() {
            Path = MakeAbsolute(zipFile).ToString(),
            Direction = Direction.dest
        }
    });}

// Publish the app to the /dist folder
Task("Package")  
    .Does(() =>
    {
        CreatePackage("Swetugg.Tix.Web");
        CreatePackage("Swetugg.Tix.Activity.Jobs");
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
    .IsDependentOn("Package");

// Executes the task specified in the target argument.
RunTarget(target);
