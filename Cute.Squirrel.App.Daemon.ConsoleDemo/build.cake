#tool MSBuild
#tool "Squirrel.Windows" 
#tool nuget:?package=vswhere // http://cakebuild.net/blog/2017/03/vswhere-and-visual-studio-2017-support
#addin Newtonsoft.Json
#addin Cake.Squirrel // https://github.com/cake-contrib/Cake.Squirrel

using Newtonsoft.Json;
using System.IO;
using System.Linq;

// Project specific variables
// NOTE: Change this for to match current project
var projectName = "Cute.Squirrel.App.Daemon.ConsoleDemo";

// Common variables
var versionAssemblyInfoPath = "./Properties/VersionAssemblyInfo.cs";
var projectPath = "./" + projectName + ".csproj";
var nuspecPath = "./" + projectName + ".nuspec";
var releasePackagesPath = "./ReleasePackages";
var projectId = projectName.Replace(".", ""); // 

Task("BumpBuildVersion")
    .Does (() => {
      var parsed = ParseAssemblyInfo(versionAssemblyInfoPath);
      var version = Version.Parse(parsed.AssemblyVersion);
      var versionBumpedString = (new Version(version.Major, version.Minor, version.Build + 1, version.Revision)).ToString();
      var assemblyInfoSettings = new AssemblyInfoSettings {
        Version = versionBumpedString,
        FileVersion = versionBumpedString
      };
      CreateAssemblyInfo(versionAssemblyInfoPath, assemblyInfoSettings);
    });

Task("ClearProject")
  .IsDependentOn("BumpBuildVersion")
  .Does(() => {
    
    var directory = System.IO.Directory.GetParent("bin\\Release\\");
    
    foreach(var f in directory.EnumerateFiles())
    {
      f.IsReadOnly = false;
      f.Delete();
    };
    
    foreach (var d in directory.EnumerateDirectories()) 
    {
        d.Delete(true);
    }
  });

Task("BuildProject")
  .IsDependentOn("ClearProject")
  .Does(() => {

    // Resolve latest (including prerelease) MSBuild
    var vsLatestPath = VSWhereLatest(new VSWhereLatestSettings() {
      ArgumentCustomization = args => args.Append("-prerelease")
    });
    var vsLatest = vsLatestPath == null ? 
      null : 
      vsLatestPath.CombineWithFilePath("./MSBuild/15.0/Bin/amd64/MSBuild.exe");

    // Configure
    var settings = new MSBuildSettings().SetConfiguration("Release")
                          .UseToolVersion(MSBuildToolVersion.VS2017)
                          .SetVerbosity(Verbosity.Minimal);
    settings.ToolPath = vsLatest;

    // Run
    MSBuild(projectPath, settings);
  });

Task("NuGetPack")
  .IsDependentOn("BuildProject")
  .Does(() => {    
    EnsureDirectoryExists(releasePackagesPath);
    NuGetPack(
      nuspecPath,
      new NuGetPackSettings {
        Id = projectId,
        Title = projectName.Replace(".", " "),
        Version = ParseAssemblyInfo(versionAssemblyInfoPath).AssemblyVersion,
        OutputDirectory = releasePackagesPath,
        ArgumentCustomization = args => args.Append("-Prop Configuration=Release")
      });
  });

Task("CreateInstaller")
  .IsDependentOn("NuGetPack")
	.Does(() => {
		var settings = new SquirrelSettings();
		settings.NoMsi = true;
		settings.Silent = true;
    settings.ArgumentCustomization = args => args.Append("-no-delta");

    // TODO Icon="icon.ico"
    // TODO ShortcutLocations = "Desktop,StartMenu"
    // TODO ProcessStartArgs=""
		
    var version = Version.Parse(ParseAssemblyInfo(versionAssemblyInfoPath).AssemblyVersion);
    var packagePath = releasePackagesPath + "/" + projectId + "." + version.Major + "." + version.Minor + "." + version.Build + ".nupkg";
    Information(packagePath);
		Squirrel(File(packagePath), settings, true, false);
	});

RunTarget("CreateInstaller");
