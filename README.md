# Toon.Serde (.NET) - Repository Scaffold

This repository contains a multi-target (.NET Standard / .NET 5 / .NET 4.8) reference implementation
of a deterministic TOON serializer and HttpClient middleware intended for LLM pipelines.

Open the solution in Visual Studio or use dotnet CLI.

## Quick start

1. Open Visual Studio -> Open Folder -> select `src/Toon.Serde`.
2. Build the solution.
3. Run tests: `dotnet test`.

## Publish (local)
`dotnet pack -c Release` -> `dotnet nuget push <nupkg> -k <API_KEY> -s https://api.nuget.org/v3/index.json`
