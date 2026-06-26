# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What This Is

A .NET 8 library (`DustInTheWind.Bcr.Toolkit`) published on NuGet that parses CSV bank statements exported from BCR (Banca Comercială Română). The solution also contains a demo console app.

## Commands

```bash
# Restore dependencies
dotnet restore ./Bcr.Toolkit.slnx --configfile ./nuget.config

# Build
dotnet build ./Bcr.Toolkit.slnx -c Release

# Run the demo (requires a statement.csv file in the demo project directory)
dotnet run --project ./sources/Bcr.Toolkit.Demo/Bcr.Toolkit.Demo.csproj
```

Publishing is done via GitHub Actions — triggered by a `vMAJOR.MINOR.PATCH` tag push.

## Architecture

The library has a two-layer design:

- **`Bcr.Toolkit` (public API)** — `StatementDocument` (extends `Collection<BankTransaction>`) is the entry point. It exposes multiple `LoadAsync` overloads (file path, string, `Stream`, `FileInfo`, `TextReader`). `BankTransaction` and `Currency` are the domain types.

- **`Bcr.Toolkit/Csv` (internal)** — `CsvStatementDocument` wraps `CsvHelper` and implements a stateful streaming read (header → transactions → footer), tracked via `CsvDocumentReadState`. `StatementRecord`/`StatementRecordMap` handle CsvHelper field mapping; `CsvFooterRecord` parses the special footer row (empty first column).

Parsing flow in `StatementDocument.LoadInternalAsync`: read header implicitly → stream transactions → on first row populate document-level fields (issuing date, account, etc.) → validate subsequent rows belong to the same statement → read footer, cross-check its `CompletionDate` against `IssuingDate`, store `EndBalance` → assert no trailing records.

Exception hierarchy: `DocumentLoadException` (base) > `HeaderLoadException`, `DataLoadException`, `FooterLoadException`.

Assembly name prefix is `DustInTheWind.` (set in `Bcr.Toolkit.csproj` via `AssemblyName`/`RootNamespace`). Shared build metadata (company, product, package tags, repo URL) lives in `Directory.Build.props`.

## Code Conventions

From `.github/copilot-instructions.md`:

- No `var` — use explicit types.
- Linq lambda parameter name: `x`.
- Prefer `new()` over `new T()` for instantiation.
- Multi-property object initializers: one property per line.
- No curly braces for single-line `if`, `for`, `using` bodies.
- No underscore prefix on C# fields.
- XML doc only on public types exposed via NuGet; skip for internal types.
- Test method naming: `Having<...>_When<...>_Then<...>`.
- Each tested method gets its own test file; all test files for one class go in a directory named after that class.
- `Assert.Throws` lambda must use a block body, not expression body.
