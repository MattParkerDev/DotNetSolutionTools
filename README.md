# Dotnet Solution Tools

Various tools to manage a C# solution.

✨ Update a solution to .NET 8 - updates csproj target versions and all Microsoft Nuget Packages

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

**warnings-as-errors** `<SolutionFilePath>` **NOT IMPLEMENTED IN CLI YET** Find projects in sln that don't have TreatWarningsAsErrors enabled. Optionally enables them.  
_options_
`-m|--add-missing` adds missing TreatWarningsAsErrors to all project files  
`-d|--enable-disabled` enables disabled TreatWarningsAsErrors in all project files  
`-a|--enable-all` enables TreatWarningsAsErrors in all project files

**clear-bin-obj** `<SolutionFolderPath>` **NOT IMPLEMENTED IN CLI YET** Deletes all bin and obj folders, and node_modules folders in the solution folder.   

