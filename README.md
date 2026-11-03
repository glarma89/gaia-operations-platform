# Gaia Operations Platform

A full-stack application for discovering and executing extensible operations on two input values, built with ASP.NET Core (.NET 10) and React.

The application exposes a REST API and a web interface. Successful executions are saved to SQL Server, and the interface displays recent history and monthly execution counts for the selected operation.

## Technology Stack

| Area | Technologies |
| --- | --- |
| Backend | C#  / .NET 10, ASP.NET Core Web API, dependency injection, OpenAPI |
| Persistence | Microsoft SQL Server, Entity Framework Core 10, EF Core migrations |
| Frontend | React, TypeScript, Vite, CSS |
| Testing | xUnit, ASP.NET Core integration tests, database repository integration tests |
| Diagnostics | ASP.NET Core HTTP logging, centralized exception handling |

## Solution Structure

```text
gaia-operations-platform/
├── backend/
│   ├── Gaia.Api/             # Controllers, API contracts, DI, logging, exception handling
│   ├── Gaia.Application/     # Operation registry and execution service
│   ├── Gaia.Domain/          # Operation contracts, models, and implementations
│   ├── Gaia.Infrastructure/  # EF Core DbContext, repository, and migrations
│   └── Gaia.Tests/           # Unit and integration tests
├── frontend/
│   └── gaia-client/          # React + TypeScript client
├── Gaia.slnx
└── README.md
```

### Architecture and Extensibility

- **Gaia.Domain:** Defines `IOperation`, operation models, domain exceptions, numeric parsing, and arithmetic/string implementations.
- **Gaia.Application:** Uses `OperationRegistry` to discover registered operations and `OperationService` to execute them.
- **Gaia.Infrastructure:** Uses `GaiaDbContext` and `OperationHistoryRepository` to persist and query operation history in SQL Server.
- **Gaia.Api:** Provides controllers, dependency injection configuration, centralized error handling, and HTTP request/response logging.
- **Gaia.Tests:** Covers operation behavior, service registration, API behavior, and database persistence.

To add an operation:

1. Implement `IOperation` in `Gaia.Domain`.
2. Define its metadata and execution behavior.
3. Register it in `backend/Gaia.Api/Extensions/DependencyInjection.cs` using `AddSingleton<IOperation, YourOperation>()`.
4. Add tests for valid and invalid inputs.

The existing operation service and API controller do not need to be changed to register another operation.

## Supported Operations

| ID | Operation | Behavior |
| --- | --- | --- |
| `add` | Addition | Adds two numbers |
| `subtract` | Subtraction | Subtracts the second number from the first |
| `multiply` | Multiplication | Multiplies two numbers |
| `divide` | Division | Divides the first number by the second |
| `concat` | Concatenation | Joins two strings |
| `equals` | String equality | Compares two strings using ordinal comparison |

Numeric operations support decimal values. Invalid numeric inputs and division by zero are rejected.

## Prerequisites

- **.NET 10 SDK**
- **Node.js and npm** (a version supported by the installed Vite release)
- **Microsoft SQL Server**, running and accessible from the backend
- **EF Core CLI (`dotnet ef`)**, version 10.x, for applying migrations
- **Git** (for cloning the repository)

The development configuration assumes a local SQL Server instance reachable as `localhost` using **Windows Authentication**. Other environments must supply their own compatible connection string.

## Getting Started

### 1. Clone and restore

```bash
git clone https://github.com/glarma89/gaia-operations-platform.git
cd gaia-operations-platform
dotnet restore Gaia.slnx
```

### 2. Configure SQL Server

The development connection string is defined in `backend/Gaia.Api/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "GaiaDatabase": "Server=localhost;Database=GaiaOperationsDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Ensure that SQL Server is running and that the Windows account starting the API has permission to connect to and create/update `GaiaOperationsDb`. If your SQL Server instance or authentication method differs, configure `ConnectionStrings:GaiaDatabase` appropriately. Do not commit passwords or other secrets to source control; use environment-specific configuration or a secret store.

### 3. Apply the database migration

From the **repository root**:

```bash
dotnet ef database update --project backend/Gaia.Infrastructure --startup-project backend/Gaia.Api
```

This applies the existing `InitialOperationHistory` migration. To inspect available migrations without changing the database:

```bash
dotnet ef migrations list --project backend/Gaia.Infrastructure --startup-project backend/Gaia.Api
```

The commands use `Gaia.Infrastructure` for migrations and `Gaia.Api` for application configuration. Ensure that the API's environment is set to `Development` (or that an equivalent connection string is configured) when invoking EF CLI outside the documented launch profile.

### 4. Start the backend

From the repository root:

```bash
dotnet run --project backend/Gaia.Api --launch-profile http
```

The HTTP development profile starts the API at **http://localhost:5148** with `ASPNETCORE_ENVIRONMENT=Development`.

### 5. Start the frontend

In a second terminal:

```bash
cd frontend/gaia-client
npm ci
npm run dev
```

Open the URL printed by Vite (typically **http://localhost:5173**). The development proxy forwards `/api` requests to `http://localhost:5148`.

### 6. Use the application

Select an operation, enter two inputs, and choose **Execute operation**. The interface displays the result, the three most recent executions of the selected operation, and its execution count for the current UTC calendar month.

> **Database dependency:** Successful execution writes to SQL Server before the API returns a successful response. The current application is not configured to run its execution workflow without an available database.

## REST API

Base URL for the documented HTTP development profile: `http://localhost:5148`.

### `GET /api/operations`

Returns available operation definitions, including their IDs and input metadata.

```http
GET http://localhost:5148/api/operations
```

### `POST /api/operations/execute`

Executes an operation and persists its input and result in the history database.

```http
POST http://localhost:5148/api/operations/execute
Content-Type: application/json

{
  "operationId": "multiply",
  "inputA": "6",
  "inputB": "7"
}
```

Example successful response:

```json
{
  "operationId": "multiply",
  "value": 42
}
```

### `GET /api/operations/{operationId}/history`

Returns history for a known operation, including:

- Up to **three most recent executions** of that operation, with inputs, result, and UTC execution timestamp.
- The **number of executions during the current UTC calendar month**.

```http
GET http://localhost:5148/api/operations/multiply/history
```

An unknown operation ID returns HTTP `404`.

### Error handling

The API uses centralized exception handling and structured error responses for operation execution errors. Relevant status codes include:

- **400 Bad Request:** Invalid operation input, such as division by zero.
- **404 Not Found:** Unknown operation.
- **500 Internal Server Error:** Unexpected server-side failure.

## Persistence and History

The `OperationHistoryRepository` stores the operation ID, two input values, serialized result, and UTC execution time. The history endpoint retrieves the latest three records for the requested operation and counts executions in the UTC month bounded by its first day and the first day of the following month.

The repository uses EF Core with SQL Server; the schema is versioned through migrations under `backend/Gaia.Infrastructure/Persistence/Migrations/`.

## Logging

ASP.NET Core HTTP logging is enabled in `Gaia.Api/Program.cs`. Its configured fields include:

- Request method and path
- Response status code
- Request and response bodies for configured text media types (including JSON)

Request and response body logging is limited to **4,096 bytes each**. The Development configuration enables the `Microsoft.AspNetCore.HttpLogging` category at `Information` level. Unexpected system errors are logged through `GlobalExceptionHandler`.

**Security:** HTTP body logging can capture sensitive inputs. The current configuration is intended for local development and testing; production deployments should restrict or redact logged content and review logging levels before enabling request/response body logging.

## Build and Test

Run backend commands from the repository root:

```bash
dotnet build Gaia.slnx --configuration Release
dotnet test Gaia.slnx --configuration Release
```

The backend test suite includes unit tests, API integration tests, and SQL Server repository integration tests. Database-dependent tests require their configured database prerequisites to be available. The most recent local test run reported **43 passing tests, 0 failures, 0 skipped**.

For a frontend production build:

```bash
cd frontend/gaia-client
npm ci
npm run build
```

The frontend build performs TypeScript checks and produces a Vite production bundle in `dist/`.

## Implementation Status

### Core features

- [x] Extensible operation engine and dependency injection
- [x] Four arithmetic and two string operations
- [x] REST API for operation discovery and execution
- [x] React/TypeScript interface for operation selection and execution
- [x] Centralized API error handling
- [x] Unit and integration tests

### Bonus features

- [x] Persist executed operations and results in SQL Server using EF Core
- [x] Display the three most recent executions of the selected operation
- [x] Display execution count for the current UTC calendar month
- [x] Log unexpected system errors
- [x] Log HTTP requests and responses in development

## Development Notes

The documented development setup uses HTTP on localhost. An HTTPS launch profile is also defined, but the Vite development proxy targets the HTTP port. The HTTP profile avoids requiring a trusted local HTTPS certificate for the documented workflow.
