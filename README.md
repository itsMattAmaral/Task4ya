# Task4ya

Task4ya is a RESTful API for task management that allows users to create boards, add tasks, assign tasks to users, and track task progress.

## Features

- **User Management**: Register, login, and manage user accounts
- **Board Management**: Create, update, and delete boards
- **Task Management**: Create, update, and delete tasks with detailed properties
- **Task Assignment**: Assign tasks to specific users
- **Task Properties**: Set priority, status, due dates, and descriptions
- **Advanced Querying**: Filter, sort, and search tasks

## Tech Stack

- **Backend**: ASP.NET Core (.NET 8)
- **Database**: PostgreSQL with Entity Framework Core
- **Architecture**: Clean Architecture with CQRS pattern (using MediatR)
- **Authentication**: JWT-based authentication
- **Documentation**: Swagger/OpenAPI

## Project Structure

```
Task4ya/
├── Task4ya.Api/              # REST API endpoints, controllers, models
├── Task4ya.Application/      # Application services, DTOs, commands, queries
├── Task4ya.Domain/           # Business entities, enums, exceptions
└── Task4ya.Infrastructure/   # Data access, repositories, external services
```

## Prerequisites

- .NET 8 SDK
- PostgreSQL database
- Docker & Docker Compose

## Getting Started

### Database Setup (Manual)

1. Install PostgreSQL
2. Create a new database called `Task4ya`
3. Update the connection string in `appsettings.json`

### Running the Application (Manual)

```bash
# Clone the repository
git clone https://github.com/itsMattAmaral/Task4ya.git
cd Task4ya
# Restore dependencies and run
cd Task4ya.Api
dotnet restore
dotnet run
```

### Running with Docker Compose

1. Ensure Docker and Docker Compose are installed.
2. Generate a self-signed HTTPS certificate:
   ```bash
   dotnet dev-certs https -ep ./https/aspnetapp.pfx -p task4ya
   ```
3. Create a `.env` file in the project root with your database credentials:
   ```env
   POSTGRES_USER=task4ya_user_admin
   POSTGRES_PASSWORD=password123
   POSTGRES_DB=Task4ya
   ```
4. Start all services:
   ```bash
   docker compose up --build -d
   ```
5. Access the API:
   - Swagger UI: [https://localhost:8443/swagger](https://localhost:8443/swagger)
   - API: [https://localhost:8443](https://localhost:8443)

> **Note:** You may see a browser warning for the self-signed certificate. This is expected for development.

## Environment Variables

- `POSTGRES_USER`, `POSTGRES_PASSWORD`, `POSTGRES_DB`: Set in `.env` for database setup
- `ASPNETCORE_URLS`: Set to `http://+:8000;https://+:8443` for Docker Compose
- `ASPNETCORE_Kestrel__Certificates__Default__Path`: Path to the HTTPS certificate
- `ASPNETCORE_Kestrel__Certificates__Default__Password`: Password for the certificate

## Migrations

Entity Framework Core migrations are applied automatically at startup.

## License

MIT
