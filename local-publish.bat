@echo on
dotnet build --configuration release
dotnet publish ./DotNetSolutionTools.App/DotNetSolutionTools.App.csproj -c release -o output -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true --self-contained false
