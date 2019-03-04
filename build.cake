// Modules
#module "nuget:?package=Cake.DotNetTool.Module&version=0.1.0"

// Tools
#tool "dotnet:?package=dotnet-sonarscanner&version=4.6.0"
#tool "nuget:?package=coveralls.io&version=1.4.2"

// Addins
#addin "nuget:?package=Cake.Git&version=0.19.0"
#addin "nuget:?package=Nuget.Core&version=2.14.0"
#addin "nuget:?package=Cake.Coveralls&version=0.9.0"

///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var currentBranch = Argument<string>("currentBranch", GitBranchCurrent("./").FriendlyName);
var prBranch = Argument<string>("prBranch", null);
var coverallsToken = Argument<string>("coverallsToken", null);
var nugetKey = Argument<string>("nugetKey", null);

///////////////////////////////////////////////////////////////////////////////
// VARIABLES
///////////////////////////////////////////////////////////////////////////////

var artifactDir = new DirectoryPath("./artifacts/");
var testDir = new DirectoryPath("./test");
var testProjectDir = testDir.Combine("Ubiety.VersionIt.Test");
var testProject = testProjectDir.CombineWithFilePath("Ubiety.VersionIt.Test.csproj");
var srcDir = new DirectoryPath("./src");
var srcProjectDir = srcDir.Combine("Ubiety.VersionIt");
var project = srcProjectDir.CombineWithFilePath("Ubiety.VersionIt.csproj");
var solution = "./Ubiety.VersionIt.sln";
var coverageFile = new FilePath("coverage.xml");
var isReleaseBuild = string.Equals(currentBranch, "master", StringComparison.OrdinalIgnoreCase) && string.IsNullOrEmpty(prBranch);
var sonarProjectKey = "ubiety_Ubiety.VersionIt";
var sonarOrganization = "ubiety";
var sonarLogin = "270d5d1be144926104cebc661863cd9c2cfac4f2";
var nugetSource = "https://api.nuget.org/v3/index.json";
var isPullRequest = !string.IsNullOrEmpty(prBranch);

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////
 
Setup(ctx =>
{
   // Executed BEFORE the first task.
   Information("Running tasks...");
});

Teardown(ctx =>
{
   // Executed AFTER the last task.
   Information("Finished running tasks.");
});

///////////////////////////////////////////////////////////////////////////////
// TASKS
///////////////////////////////////////////////////////////////////////////////

Task("Clean")
.Does(() => {
    if (DirectoryExists(artifactDir))
    {
        DeleteDirectory(
            artifactDir,
            new DeleteDirectorySettings
            {
                Recursive = true,
                Force = true
            });
    }

    CreateDirectory(artifactDir);
    DotNetCoreClean(solution);
});

Task("Restore")
.Does(() => {
    DotNetCoreRestore(solution);
});

Task("Build")
.IsDependentOn("Clean")
.IsDependentOn("Restore")
.Does(() => {
    DotNetCoreBuild(
        solution,
        new DotNetCoreBuildSettings
        {
            Configuration = configuration
        }
    );
});

Task("Test")
.Does(() => {
    var settings = new DotNetCoreTestSettings
    {
        ArgumentCustomization = args => args
            .Append("/p:CollectCoverage=true")
            .Append("/p:CoverletOutputFormat=opencover")
            .Append($"/p:CoverletOutput=./{coverageFile}")
            .Append("/p:Exclude=\"[xunit.*]*\"")
    };

    DotNetCoreTest(testProject.FullPath, settings);
    MoveFile(testProjectDir.CombineWithFilePath(coverageFile), artifactDir.CombineWithFilePath(coverageFile));
});

if (isPullRequest)
{
    Task("SonarBegin")
    .Does(() => {
        DotNetCoreTool("sonarscanner", new DotNetCoreToolSettings {
           ArgumentCustomization = args => args
                .Append("begin")
                .Append($"/k:\"{sonarProjectKey}\"")
                .Append($"/o:\"{sonarOrganization}\"")
                .Append("/d:sonar.host.url=\"https://sonarcloud.io\"")
                .Append($"/d:sonar.login=\"{sonarLogin}\"")
                .Append($"/d:sonar.cs.opencover.reportsPaths={artifactDir.CombineWithFilePath(coverageFile)}")
                .Append($"/d:sonar.pullrequest.branch=\"{prBranch}\"")
                .Append($"/d:sonar.pullrequest.key=\"{EnvironmentVariable("APPVEYOR_PULL_REQUEST_NUMBER")}\"")
                .Append($"/d:sonar.pullrequest.base=\"{EnvironmentVariable("APPVEYOR_REPO_BRANCH")}\"")
        });
    });
}
else
{
    Task("SonarBegin")
    .Does(() => {
       DotNetCoreTool("sonarscanner", new DotNetCoreToolSettings {
           ArgumentCustomization = args => args
                .Append("begin")
                .Append($"/k:\"{sonarProjectKey}\"")
                .Append($"/o:\"{sonarOrganization}\"")
                .Append("/d:sonar.host.url=\"https://sonarcloud.io\"")
                .Append($"/d:sonar.login=\"{sonarLogin}\"")
                .Append($"/d:sonar.cs.opencover.reportsPaths={artifactDir.CombineWithFilePath(coverageFile)}")
       });
    });
}

Task("SonarEnd")
.Does(() => {
    DotNetCoreTool("sonarscanner", new DotNetCoreToolSettings {
        ArgumentCustomization = args => args
            .Append("end")
            .Append($"/d:sonar.login=\"{sonarLogin}\"")
    });
});

Task("UploadCoverage")
.IsDependentOn("Test")
.Does(() => {
    CoverallsIo(artifactDir.CombineWithFilePath(coverageFile), new CoverallsIoSettings {
        RepoToken = coverallsToken
    });
});

Task("Package")
.Does(() => {
    DotNetCorePack(project.FullPath, new DotNetCorePackSettings {
        NoBuild = true,
        OutputDirectory = artifactDir
    });
});

Task("Publish")
.IsDependentOn("Package")
.Does(() => {
    var settings = new DotNetCoreNuGetPushSettings {
        Source = nugetSource,
        ApiKey = nugetKey
    };

    var packages = GetFiles(artifactDir.CombineWithFilePath("*.nupkg").FullPath);

    foreach (var package in packages)
    {
        if (!IsPublished(package))
        {
            Information($"Publishing \"{package}\"");
            DotNetCoreNuGetPush(package.FullPath, settings);
        }
        else
        {
            Information($"Package \"{package}\" already published. Skipping.");
        }
    }
});

Task("Version")
.Does(() => {
    DotNetCoreTool("versionit");
});

Task("PushChanges")
.Does(() => {
    GitPush("./");
});

Task("Sonar")
.IsDependentOn("SonarBegin")
.IsDependentOn("BuildAndTest")
.IsDependentOn("SonarEnd");

Task("BuildAndTest")
.IsDependentOn("Build")
.IsDependentOn("Test");

Task("CompleteWithoutPublish")
.IsDependentOn("BuildAndTest")
.IsDependentOn("UploadCoverage");

if (isReleaseBuild)
{
    Task("Complete")
    .IsDependentOn("Version")
    .IsDependentOn("CompleteWithoutPublish")
    .IsDependentOn("Publish")
    .IsDependentOn("PushChanges");
}
else
{
    Task("Complete")
    .IsDependentOn("Sonar")
    .IsDependentOn("UploadCoverage");
}

Task("Default")
.IsDependentOn("Complete");

RunTarget(target);

private bool IsPublished(FilePath packagePath)
{
    var package = new NuGet.ZipPackage(packagePath.FullPath);

    var latestPublishedVersions = NuGetList(package.Id, new NuGetListSettings {
        Prerelease = true
    });

    return latestPublishedVersions.Any(p => package.Version.Equals(new NuGet.SemanticVersion(p.Version)));
}
