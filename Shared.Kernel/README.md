# Shared.Kernel

Shared kernel for DirectoryService projects.

## Contents

- `Shared.Kernel` — `CreatedObjectGuid`.
- `Shared.Kernel.ErrorsShared` — `Error`, `Errors`, `GeneralErrors`.
- `Shared.Kernel.Results` — `Envelope`, `EndPointResult`, `ErrorsResult`, `SuccessResult`.
- `Shared.Kernel.Exceptions` — `BadRequestException`, `ConflictException`, `NotFoundException`.
- `Shared.Kernel.Statistics` — `Statistica`.
- `Shared.Kernel.GlobalStatisticsClass` — `GlobalStatistics`.

## Install

```bash
dotnet add package Shared.Kernel
```

## Requirements

- .NET 9
- ASP.NET Core shared framework

## Build package

```bash
dotnet pack -c Release
```

The package is written to `bin/Release/Shared.Kernel.<version>.nupkg`.
