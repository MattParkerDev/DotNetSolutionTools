# Solution Parity Checker

Various tools to manage a C# solution.

## CLI

**compare** `<SolutionFolderPath>` `<SolutionFilePath>`  
_options_ 
`-l --logprojectfiles`  logs all project files found in folder

implicit-usings `<SolutionFilePath>`  
_options_
`-m|--add-missing` adds missing implicit usings to all project files  
`-d|--enable-disabled` enables disabled implicit usings in all project files  
`-a|--enable-all` enables implicit usings in all project files

**format-csproj**  
`--folder <SolutionFolderPath>` or
`--sln <SolutionFilePath>` or
`--project <CsprojFilePath>`

## App

TODO