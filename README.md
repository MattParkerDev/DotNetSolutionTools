# Dotnet Solution Tools

Various tools to manage a C# solution.

## CLI

**compare** `<SolutionFolderPath>` `<SolutionFilePath>` Finds any missing C# projects in the solution file compared to the folder.   
_options_ 
`-l --logprojectfiles`  logs all project files found in folder

**implicit-usings** `<SolutionFilePath>` Find projects that don't have implicit usings enabled. Optionally enables them.  
_options_
`-m|--add-missing` adds missing implicit usings to all project files  
`-d|--enable-disabled` enables disabled implicit usings in all project files  
`-a|--enable-all` enables implicit usings in all project files

**format-csproj** Formats a C# project file.  
`--folder <SolutionFolderPath>` or
`--sln <SolutionFilePath>` or
`--project <CsprojFilePath>`

## App

TODO