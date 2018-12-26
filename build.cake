#tool nuget:?package=NUnit.ConsoleRunner&version=3.4.0
#tool nuget:?package=vswhere
#addin "Cake.Incubator"
//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

var outputDirectory = Argument("outputDirectory", string.Empty);
var cacheDirectory = Argument("cacheDirectory", string.Empty);

var deathClockData = Argument("deathClockData", string.Empty);

var tmdbApiKey = Argument("tmdbApiKey", string.Empty);

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var buildDir = Directory(configuration);

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectories("./**/bin/**");
});

Task("Restore-NuGet-Packages")
    .Does(() =>
{
    DotNetCoreRestore("./DeathClock.sln");
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
    DotNetCoreBuild("./DeathClock.sln", new DotNetCoreBuildSettings {
    Verbosity = DotNetCoreVerbosity.Minimal,
    Configuration = configuration
    });

    var publishSettings = new DotNetCorePublishSettings
    {
        Configuration = configuration
    };
    DotNetCorePublish("./DeathClock.Web.UI/DeathClock.Web.UI.csproj", publishSettings);
});

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
{
    DotNetCoreTest("DeathClock.Test/DeathClock.Test.csproj");
});

Task("TestRun")
    .IsDependentOn("Build")
    .IsDependentOn("Test")
    .Does(() =>
{
    var command = string.Empty;
    if (!string.IsNullOrEmpty(outputDirectory)) {
        command += $"-outputDirectory {outputDirectory}";
    }
    if (!string.IsNullOrEmpty(cacheDirectory)) {
        command += $" -cacheDirectory {cacheDirectory}";
    }

    command += " -list List_of_Scots";

    DotNetCoreExecute($"./DeathClock/bin/{buildDir}/netcoreapp2.1/DeathClock.dll", command);
});

Task("Run")
    .IsDependentOn("Build")
    .IsDependentOn("Test")
    .Does(() =>
{
    var command = string.Empty;
    if (!string.IsNullOrEmpty(outputDirectory)) {
        command += $"-outputDirectory {outputDirectory}";
    }
    if (!string.IsNullOrEmpty(cacheDirectory)) {
        command += $" -cacheDirectory {cacheDirectory}";
    }

    command += " -list List_of_English_people List_of_Scots List_of_Welsh_people List_of_Irish_people Lists_of_Americans";

    DotNetCoreExecute($"./DeathClock/bin/{buildDir}/netcoreapp2.1/DeathClock.dll", command);
});

Task("Run-Tmdb")
    .IsDependentOn("Build")
    .IsDependentOn("Test")
    .Does(() =>
{
    var command = "tmdb";
    if (!string.IsNullOrEmpty(tmdbApiKey)) {
        command += $" -tmdbApiKey {tmdbApiKey}";
    }
    
    DotNetCoreExecute($"./DeathClock/bin/{buildDir}/netcoreapp2.1/DeathClock.dll", command);
});

Task("Run-All")
    .IsDependentOn("Run")
    .IsDependentOn("Run-Tmdb")
    .Does(() => { });

Task("Web-UI")
    .IsDependentOn("Build")
    .Does(() =>
{
    var publishDirectory = $"./DeathClock.Web.UI/bin/{buildDir}/netcoreapp2.1/publish";
    var executeSettings = new DotNetCoreExecuteSettings
    {
        WorkingDirectory = publishDirectory
    };

    if (string.IsNullOrEmpty(deathClockData)) {
        throw new Exception("deathClockData must have json paths.");
    }

    DotNetCoreExecute($"{publishDirectory}/DeathClock.Web.UI.dll", $"--DeathClockData {deathClockData}", executeSettings);
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Test");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
