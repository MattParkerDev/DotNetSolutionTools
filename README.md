# Dotnet Solution Tools

Various tools to manage a C# solution.

✨ Update a solution to .NET 8 - updates csproj target versions and all Microsoft Nuget Packages

## CLI

**compare** `<SolutionFolderPath>` `<SolutionFilePath>` Finds any missing C# projects in the solution file compared to the folder.   
_options_ 
`-l --logprojectfiles`  logs all project files found in folder

**implicit-usings** `<SolutionFilePath>` Find projects that don't have implicit usings enabled. Optionally enables them.  
_options_
`-m|--add-missing` adds missing implicit usings to all project files  
`-d|--enable-disabled` enables disabled implicit usings in all project files  
`-a|--enable-all` enables implicit usings in all project files

**format-csproj** Formats a C# project file(s).  
`--folder <SolutionFolderPath>` or
`--sln <SolutionFilePath>` or
`--project <CsprojFilePath>`

## App
The app feels quite self explanatory :)

![image](https://github.com/MattParkerDev/DotNetSolutionTools/assets/61717342/4a5f49d4-bf9f-4940-bc8b-06df46ecb972)

