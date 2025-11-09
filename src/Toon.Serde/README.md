# Toon.Serde (.NET)

Minimal deterministic TOON serializer for .NET.

## Install
When published:
```
dotnet add package Toon.Serde --version 0.1.0
```

## Usage (basic)
```csharp
using Newtonsoft.Json.Linq;
using Toon.Serde;

var json = JObject.Parse(@"{"user":{"id":123,"name":"Akira"}, "tags": ["agent","llm"]}");
var toon = ToonConverter.ToToon(json, new ToonOptions { Indent = 2, StableKeys = true });
Console.WriteLine(toon);
```

## HttpClient integration
Wrap your LLM HttpClient with `ToonHttpClientHandler` to automatically convert outgoing JSON bodies to TOON and set header `X-TOON: 1`.

## Publishing
- Ensure `PackageId` and `Version` in csproj are set.
- Add `NUGET_API_KEY` to GitHub Secrets.
- Push annotated tag `vX.Y.Z` to publish via CI.

## Roadmap
- Add robust `FromToon` parser (round-trip).
- Streaming encoder/decoder.
- Token estimator & telemetry.
