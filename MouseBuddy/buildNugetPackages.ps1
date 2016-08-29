rm *.nupkg
nuget pack .\MouseBuddy.nuspec -IncludeReferencedProjects -Prop Configuration=Release
nuget push *.nupkg