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
- Docker (optional, for containerization)

## Getting Started

### Database Setup

1. Install PostgreSQL
2. Create a new database called `Task4ya`
3. Update the connection string in `appsettings.json`

### Running the Application

```bash
# Clone the repository
git clone https://github.com/itsMattAmaral/Task4ya.git
cd Task4ya

# Build the solution
dotnet build

# Run migrations
dotnet ef database update --project Task4ya.Infrastructure --startup-project Task4ya.Api

# Run the application
cd Task4ya.Api
dotnet run
```

The API will be available at `https://localhost:5001` or `http://localhost:5000`

## API Endpoints

### Authentication

- `POST /api/Auth/register` - Register a new user
- `POST /api/Auth/login` - Login and get JWT token

### Boards

- `GET /api/Board` - Get all boards
- `GET /api/Board/{id}` - Get board by ID
- `POST /api/Board` - Create a new board
- `PUT /api/Board/{id}` - Update a board
- `DELETE /api/Board/{id}` - Delete a board

### Tasks

- `GET /api/TaskItem` - Get all tasks (with pagination, sorting, filtering)
- `GET /api/TaskItem/{id}` - Get task by ID
- `POST /api/TaskItem` - Create a new task
- `PUT /api/TaskItem/{id}` - Update a task
- `PATCH /api/TaskItem/{id}/status` - Update task status
- `PATCH /api/TaskItem/{id}/priority` - Update task priority
- `PATCH /api/TaskItem/{id}/DueDate` - Update task due date
- `PATCH /api/TaskItem/{id}/assignee` - Update task assignee
- `DELETE /api/TaskItem/{id}` - Delete a task

## Authentication

All endpoints except for login and register require authentication using JWT Bearer token:

```
Authorization: Bearer <your_token>
```

## Error Handling

The API returns appropriate HTTP status codes:

- `200 OK` - Success
- `201 Created` - Resource created
- `400 Bad Request` - Invalid input
- `401 Unauthorized` - Authentication required
- `404 Not Found` - Resource not found
- `500 Internal Server Error` - Server error

## Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgments

- Built with ASP.NET Core and Entity Framework Core
- Uses MediatR for implementing the CQRS pattern
- Uses clean architecture principles for maintainability and testability
