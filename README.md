# Gaia Operations Platform

A full-stack application for executing extensible operations on two input values, built with ASP.NET Core and React.

## Project Overview

Gaia Operations Platform provides a REST API and a web interface for selecting and executing operations.

The application is designed around an extensible operation engine. New operations can be added without modifying the existing operation implementations, execution engine, or API controllers.

## Technology Stack

**Backend**
- C# / .NET 10
- ASP.NET Core Web API
- Dependency Injection
- OpenAPI

**Frontend**
- React
- TypeScript
- Vite
- CSS

**Testing**
- xUnit
- ASP.NET Core integration tests

**Planned Bonus Features**
- Microsoft SQL Server
- Entity Framework Core
- Operation history and statistics
- HTTP request/response logging

## Solution Structure

```text
gaia-operations-platform/
├── backend/
│   ├── Gaia.Api/
│   ├── Gaia.Application/
│   ├── Gaia.Domain/
│   ├── Gaia.Infrastructure/
│   └── Gaia.Tests/
├── frontend/
│   └── gaia-client/
├── docs/
├── Gaia.slnx
└── README.md
```

### Architecture

**Gaia.Domain**

Contains the operation contracts, domain models, exceptions, and implementations of arithmetic and string operations.

**Gaia.Application**

Contains `OperationRegistry` and `OperationService`, which manage and execute registered operations.

**Gaia.Api**

Exposes the REST API, configures dependency injection, and handles application exceptions.

**Gaia.Infrastructure**

Reserved for infrastructure-related functionality, including the planned database integration.

**Gaia.Tests**

Contains unit and integration tests.

## Supported Operations

| ID | Operation | Description |
|---|---|---|
| `add` | Addition | Adds two numbers |
| `subtract` | Subtraction | Subtracts the second number from the first |
| `multiply` | Multiplication | Multiplies two numbers |
| `divide` | Division | Divides the first number by the second |
| `concat` | Concatenation | Combines two strings |
| `equals` | String Equality | Compares two strings using ordinal comparison |

Numeric operations support decimal values.

Division by zero and invalid numeric inputs are handled by the API.

## API Endpoints

### GET /api/operations

Returns the list of available operations, including their identifiers, names, descriptions, and input types.

Example:

```http
GET http://localhost:5148/api/operations
```

### POST /api/operations/execute

Executes the specified operation using two input values.

Example request:

```http
POST http://localhost:5148/api/operations/execute
Content-Type: application/json
```

```json
{
  "operationId": "multiply",
  "inputA": "6",
  "inputB": "7"
}
```

Example response:

```json
{
  "operationId": "multiply",
  "value": 42
}
```

### Error Handling

The API uses centralized exception handling and returns structured error responses.

Examples:

- HTTP 400 — invalid numeric input or division by zero
- HTTP 404 — unknown operation
- HTTP 500 — unexpected server error

## Getting Started

### Prerequisites

- .NET 10 SDK
- Node.js and npm
- Git

Microsoft SQL Server is not required to run the currently implemented core application.

### 1. Clone the Repository

```bash
git clone https://github.com/glarma89/gaia-operations-platform.git
cd gaia-operations-platform
```

Replace `https://github.com/glarma89/gaia-operations-platform.git` with the actual GitHub repository URL.

### 2. Start the Backend

From the repository root:

```bash
dotnet restore Gaia.slnx
dotnet run --project backend/Gaia.Api --launch-profile http
```

The backend listens at:

```text
http://localhost:5148
```

### 3. Start the Frontend

Open a second terminal:

```bash
cd frontend/gaia-client
npm install
npm run dev
```

Open the local URL displayed by Vite, typically:

```text
http://localhost:5173
```

The Vite development server proxies `/api` requests to the backend.

### 4. Use the Application

1. Select an operation.
2. Enter the first and second values.
3. Click **Execute operation**.
4. View the result or an error message.

## Testing

### Backend Build

```bash
dotnet build Gaia.slnx --configuration Release
```

### Backend Tests

```bash
dotnet test Gaia.slnx --configuration Release
```

The test suite includes unit tests for the operation engine and integration tests for the REST API.

### Frontend Production Build

```bash
cd frontend/gaia-client
npm run build
```

The frontend build includes TypeScript validation and Vite production bundling.

## Extensibility

The application uses the `IOperation` interface to define a common contract for all operations.

To add a new operation:

1. Create a new class implementing `IOperation`.
2. Provide its operation definition and execution logic.
3. Register the new implementation in `DependencyInjection.cs` using `AddSingleton<IOperation, YourOperation>()`.
4. Add unit tests for the new operation.

The operation registry automatically collects registered implementations.

The operation service and API controllers do not need to be modified.

## Implementation Status

### Core Requirements — Implemented

- Extensible operation architecture
- Arithmetic and string operations
- REST API for operation discovery and execution
- React-based user interface
- Input handling and result display
- Centralized API error handling
- Backend unit and integration tests
- Responsive frontend layout

### Bonus Requirements — Planned

- Persist executed operations and results in SQL Server
- Display the three most recent operations of the same type
- Display the number of operations of the same type executed during the current calendar month
- Log system errors
- Log HTTP requests and responses

## Development Notes

The current development configuration uses HTTP on localhost.

HTTPS certificate trust configuration is not required for running the application using the documented HTTP development profile.
