# Simple Landscaping API

A minimal ASP.NET Core Web API for managing landscaping project estimates.

## Features

- **Simple CRUD Operations**: Create, read, update, and delete project estimates
- **In-Memory Database**: Uses Entity Framework Core with in-memory database
- **Swagger Documentation**: Auto-generated API documentation
- **Basic Tests**: Unit tests for core functionality

## Model

The `ProjectEstimate` model contains:
- `Id`: Unique identifier
- `ProjectName`: Name of the landscaping project
- `ClientName`: Client's name
- `ClientEmail`: Client's email address
- `EstimatedCost`: Project cost estimate
- `Status`: Current status (Draft, Pending, Approved)
- `CreatedDate`: When the estimate was created

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/ProjectEstimates` | Get all project estimates |
| GET | `/api/ProjectEstimates/{id}` | Get specific project estimate |
| POST | `/api/ProjectEstimates` | Create new project estimate |
| PUT | `/api/ProjectEstimates/{id}` | Update existing project estimate |
| DELETE | `/api/ProjectEstimates/{id}` | Delete project estimate |

## Getting Started

1. **Run the API**
   ```bash
   dotnet run --project SimpleApi
   ```

2. **Access the API**
   - API Base URL: `http://localhost:5274` (or check console for actual port)
   - Swagger UI: `http://localhost:5274/swagger`

3. **Run Tests**
   ```bash
   dotnet test
   ```

## Sample Data

The API comes with 3 sample project estimates:
- Garden Design ($15,000) - John Smith
- Pool Renovation ($25,000) - Jane Doe  
- Commercial Landscaping ($45,000) - ABC Corp

## Example Usage

### Get All Estimates
```bash
curl http://localhost:5274/api/ProjectEstimates
```

### Create New Estimate
```bash
curl -X POST http://localhost:5274/api/ProjectEstimates \
  -H "Content-Type: application/json" \
  -d '{
    "projectName": "Backyard Makeover",
    "clientName": "Alice Johnson",
    "clientEmail": "alice@email.com",
    "estimatedCost": 18000,
    "status": "Draft"
  }'
```

## Technology Stack

- .NET 9.0
- ASP.NET Core Web API
- Entity Framework Core (In-Memory)
- xUnit (Testing)
- Swagger/OpenAPI

## Project Structure

```
SimpleApi/
├── Controllers/
│   └── ProjectEstimatesController.cs
├── Data/
│   └── ProjectContext.cs
├── Models/
│   └── ProjectEstimate.cs
└── Program.cs

SimpleApi.Tests/
└── ProjectEstimatesControllerTests.cs
```

This is a minimal, interview-friendly project that demonstrates core ASP.NET Core concepts without overwhelming complexity.