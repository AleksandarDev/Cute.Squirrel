#tool MSBuild
#tool "Squirrel.Windows" 
#addin Newtonsoft.Json
#addin Cake.Squirrel

using Newtonsoft.Json;

// Project specific variables
// NOTE: Change this for to match current project
var projectName = "Cute.Squirrel.Babbler.SignalR.Server.SelfHostConsole";

// Common variables
var versionAssemblyInfoPath = "./Properties/VersionAssemblyInfo.cs";
var projectPath = "./" + projectName + ".csproj";
var nuspecPath = "./" + projectName + ".nuspec";
var releasePackagesPath = "./ReleasePackages";

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

Task("BuildProject")
  .IsDependentOn("BumpBuildVersion")
  .Does(() => {
    MSBuild(
      projectPath,
      settings => settings.SetConfiguration("Release")
                          .SetVerbosity(Verbosity.Minimal));
  });

Task("NuGetPack")
  .IsDependentOn("BuildProject")
  .Does(() => {    
    EnsureDirectoryExists(releasePackagesPath);
    NuGetPack(
      nuspecPath,
      new NuGetPackSettings {
        Id = projectName,
        Title = projectName,
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

    // TODO Icon="icon.ico"
    // TODO ShortcutLocations = "Desktop,StartMenu"
    // TODO ProcessStartArgs=""
		
    var version = Version.Parse(ParseAssemblyInfo(versionAssemblyInfoPath).AssemblyVersion);
    var packagePath = releasePackagesPath + "/" + projectName + "." + version.Major + "." + version.Minor + "." + version.Build + ".nupkg";
    Information(packagePath);
		Squirrel(File(packagePath), settings, true, false);
	});

RunTarget("CreateInstaller");