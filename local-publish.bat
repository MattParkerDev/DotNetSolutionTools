@echo on
dotnet build -c Release
dotnet publish ./src/DotNetSolutionTools.App/DotNetSolutionTools.App.csproj -c Release -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true --self-contained false
