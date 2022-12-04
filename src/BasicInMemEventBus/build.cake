#tool "dotnet:?package=GitVersion.Tool&version=5.11.1"
#tool "nuget:?package=NuGet.CommandLine&version=6.2.1"

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
string version = string.Empty;
string informationalVersion = string.Empty;
//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")    
    .Description("Cleaning projects")
    .Does(() =>
{
    DotNetClean($"./");
    DotNetClean($"../BasicInMemEventBus.Tests/");
});


Task("Restore")
    .IsDependentOn("Clean")
    .Description("Restoring the solution dependencies")
    .Does(() => {
           var projects = GetFiles("../**/*.csproj");

              foreach(var project in projects )
              {
                  Information($"Building { project.ToString()}");
                  DotNetRestore(project.ToString());
              } 
});


Task("Version")
    .IsDependentOn("Restore")
    .Does(() => {
   var result = GitVersion(new GitVersionSettings {
        //ConfigFile = new FilePath("../../.github/gitversion.yml"),
        UpdateAssemblyInfo = true
    });
    
    version = result.NuGetVersionV2;
    informationalVersion = result.InformationalVersion;
    Information($"Version: { version }");
    Information($"Informational Version: { informationalVersion }");
});


Task("Build")
    .IsDependentOn("Version")
    .Does(() => {
     var buildSettings = new DotNetBuildSettings {
                        Configuration = configuration,
                        MSBuildSettings = new DotNetMSBuildSettings()
                                                      .WithProperty("Version", version)
                                                      .WithProperty("Copyright", $"Copyright Db Coder {DateTime.Now.Year}")
                                                      .WithProperty("AssemblyVersion", version)
                                                      .WithProperty("FileVersion", version)
                                                      .WithProperty("InformationalVersion", informationalVersion)
                       };
     var projects = GetFiles("../**/*.csproj");
     foreach(var project in projects )
     {
         Information($"Building {project.ToString()}");
         DotNetBuild(project.ToString(), buildSettings);
     }
});


Task("Test")
    .IsDependentOn("Build")
    .Does(() => {

       var testSettings = new DotNetTestSettings  {
                                  Configuration = configuration,
                                  NoBuild = true,
                              };
     var projects = GetFiles("../BasicInMemEventBus.Tests/*/*.csproj");
     foreach(var project in projects )
     {
       Information($"Running Tests : { project.ToString()}");
       DotNetTest(project.ToString(), testSettings );
     }
});


Task("Pack")
 .IsDependentOn("Test")
 .Does(() => {
 
   var settings = new DotNetPackSettings
    {
        Configuration = configuration,
        OutputDirectory = "../.artifacts",
        NoBuild = true,
        NoRestore = true,
        MSBuildSettings = new DotNetMSBuildSettings()
                        .WithProperty("PackageVersion", version)
                        .WithProperty("Copyright", $"Copyright Db Coder {DateTime.Now.Year}")
                        .WithProperty("Version", version)
                        .WithProperty("InformationalVersion", informationalVersion)
                        .WithProperty("PackageTags", "stuff")
                        .WithProperty("Description", "A very basic in-memory event bus")
    };
    
    DotNetPack("./BasicInMemEventBus.csproj", settings);
 });


Task("PublishGithub")
  .IsDependentOn("Pack")
  .Does(context => {
  if (BuildSystem.GitHubActions.IsRunningOnGitHubActions)
   {
      foreach(var file in GetFiles("../.artifacts/*.nupkg"))
      {
        Information("Publishing {0}...", file.GetFilename().FullPath);
        DotNetNuGetPush(file, new DotNetNuGetPushSettings {
              ApiKey = EnvironmentVariable("GITHUB_TOKEN"),
              Source = "https://nuget.pkg.github.com/DBCoderAtLarge/index.json"
        });
      } 
   } 
 }); 

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

Task("Default")
       .IsDependentOn("Clean")       
       .IsDependentOn("Restore")
       .IsDependentOn("Version")
       .IsDependentOn("Build")
       .IsDependentOn("Test")
       .IsDependentOn("Pack")
       .IsDependentOn("PublishGithub");

RunTarget(target);