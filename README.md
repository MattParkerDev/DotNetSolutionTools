# Dotnet Solution Tools

Various tools to manage a C# solution.

✨ Update a solution to .NET 8 - updates csproj target versions and all Microsoft Nuget Packages. Ensure you update the global.json to use the latest SDK version, as this is not done for you.

## App
The app feels quite self explanatory :)
![image](https://github.com/MattParkerDev/DotNetSolutionTools/assets/61717342/f2d581da-5f28-4dfd-8a33-32c58995a6bf)

## CLI

**compare** `<SolutionFolderPath>` `<SolutionFilePath>` Finds any missing C# projects in the solution file compared to the folder.   
_options_
`-l --logprojectfiles`  logs all project files found in folder

**format-csproj** Formats a C# project file(s).  
`--folder <SolutionFolderPath>` or
`--sln <SolutionFilePath>` or
`--project <CsprojFilePath>`

**implicit-usings** `<SolutionFilePath>` Find projects in sln that don't have ImplicitUsings enabled. Optionally enables them.  
_options_
`-m|--add-missing` adds missing implicit usings to all project files  
`-d|--enable-disabled` enables disabled implicit usings in all project files  
`-a|--enable-all` enables implicit usings in all project files

**warnings-as-errors** `<SolutionFilePath>` Find projects in sln that don't have TreatWarningsAsErrors enabled. Optionally enables them.  
_options_
`-m|--add-missing` adds missing TreatWarningsAsErrors to all project files

**clear-bin-obj** `<SolutionFolderPath>` Deletes all bin and obj folders, and node_modules folders in the solution folder.   

**update-sln-net80** `<SolutionFilePath>` Updates all projects and Microsoft NuGet packages in sln to .NET 8.0.

**update-csproj-net80** `<CsprojFilePath>` Updates project and Microsoft NuGet packages to .NET 8.0.  
