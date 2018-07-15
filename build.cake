#tool nuget:?package=NUnit.ConsoleRunner&version=3.4.0
#tool nuget:?package=vswhere
#addin "Cake.Incubator"
//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

var outputDirectory = Argument("outputDirectory", string.Empty);

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

    command += " -list List_of_English_people List_of_Scots List_of_Welsh_people List_of_Irish_people Lists_of_Americans";

    DotNetCoreExecute($"./DeathClock/bin/{buildDir}/netcoreapp2.1/DeathClock.dll", command);
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
