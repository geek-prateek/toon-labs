## Toon.Serde — Learning-Oriented TOON Serializer for .NET

Toon.Serde is a lightweight, open-source implementation of TOON (Token-Oriented Object Notation) designed specifically for learning, experimentation, and understanding how TOON works under the hood.

This project demonstrates the core mechanics of converting JSON → TOON and TOON → JSON inside a .NET environment.
It is not positioned as an official or production-grade serializer, but rather as a reference implementation for developers exploring token-efficient data formats for AI/LLM workflows.

🚀 What This Package Covers

Deterministic JSON → TOON serialization

Indentation-based object formatting

Array length notation (tags[2]:)

Minimal quoting rules for cleaner LLM prompts

Optional stable key ordering for deterministic output

Simple HttpClient handler to send TOON instead of JSON

🎯 Why This Exists

Modern LLM pipelines often struggle with:

High token costs

Noisy JSON structures

Limited context windows

TOON addresses these with a syntax that is:

Compact

LLM-friendly

Human-readable

Token-efficient

This repository helps developers learn:

How TOON encoding works

How to build their own serializer

How TOON can improve LLM-based agents and prompt workflows

📦 NuGet Package

Install via:

dotnet add package Toon.Serde


NuGet link:
https://www.nuget.org/packages/Toon.Serde

🧪 Example Usage
using Newtonsoft.Json.Linq;
using Toon.Serde;

var json = JObject.Parse(@"{ ""user"": { ""id"": 123, ""name"": ""Akira"" }, ""tags"": [""agent"", ""llm""] }");
string toon = ToonConverter.ToToon(json);

Console.WriteLine(toon);


Output:

user:
  id: 123
  name: Akira
tags[2]:
  agent
  llm

📘 Deep Dive Blog

For a full explanation of TOON, comparisons with JSON, and real use cases, read the blog post:

“TOON: The Data Format Changing How AI Agents Think”

[(Medium Blog)](https://medium.com/@prateek.dbg/toon-the-data-format-changing-how-ai-agents-think-30c3d1f7b5bc)

🤝 Contributing

This project is intentionally simple and transparent.
Feel free to open issues, submit PRs, or fork it for educational use.
