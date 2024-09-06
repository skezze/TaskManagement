Task Management System
Overview
The Task Management System is a comprehensive solution for managing tasks and user authentication. It includes a set of APIs for user registration, login, task creation, updates, and retrieval.

Architecture
Design Choices
Architecture: The system is designed using a layered architecture with separation of concerns. It includes:

Controllers: Handle HTTP requests and responses.
Services: Contain business logic.
Repositories: Interface with the database.
DTOs: Data Transfer Objects for data encapsulation and validation.
Authentication: Uses JWT (JSON Web Tokens) for secure user authentication and authorization.

Database: PostgreSQL is used as the database for storing user and task information.

Logging: Serilog is used for logging, configured to write logs to files for better traceability.

Deployment: Docker Compose is used for local deployment, ensuring a consistent environment.

Setup Instructions
Prerequisites
Docker (for running Docker containers)
Docker Compose (for managing multi-container Docker applications)
Running the Project Locally
Clone the Repository:

bash
Copy code
git clone https://github.com/your-repo/task-management-system.git
cd task-management-system
Configure Environment Variables: Create a .env file in the root directory with the following content:

env
Copy code
POSTGRES_USER=your_postgres_user
POSTGRES_PASSWORD=your_postgres_password
POSTGRES_DB=task_management_db
SECRET_KEY=your_jwt_secret_key
Build and Start Containers:

bash
Copy code
docker-compose up --build
Access the Application:

The API will be available at http://localhost:5000.
Stopping the Application: To stop the application, press CTRL+C or run:

bash
Copy code
docker-compose down
API Documentation
User Endpoints
Register User

Endpoint: POST /users/register
Request Body:
json
Copy code
{
  "username": "string",
  "password": "string",
  "email": "string"
}
Response: 201 Created on success, 400 Bad Request on error.
Login User

Endpoint: POST /users/login
Request Body:
json
Copy code
{
  "usernameOrEmail": "string",
  "password": "string"
}
Response: 200 OK with JWT token on success, 401 Unauthorized on error.
Task Endpoints
Create Task

Endpoint: POST /tasks
Request Body:
json
Copy code
{
  "title": "string",
  "description": "string",
  "dueDate": "2024-01-01T00:00:00Z",
  "status": "Pending",
  "priority": "High"
}
Response: 201 Created on success, 400 Bad Request on error.
Get Tasks

Endpoint: GET /tasks
Query Parameters: status, dueDate, priority, sortBy, descending, pageNumber, pageSize
Response: 200 OK with a list of tasks.
Get Task by ID

Endpoint: GET /tasks/{id}
Response: 200 OK with task details, 404 Not Found if the task is not found.
Update Task

Endpoint: PUT /tasks/{id}
Request Body:
json
Copy code
{
  "title": "string",
  "description": "string",
  "dueDate": "2024-01-01T00:00:00Z",
  "status": "Pending",
  "priority": "High"
}
Response: 200 OK on success, 400 Bad Request on error.
Delete Task

Endpoint: DELETE /tasks/{id}
Response: 204 No Content on success, 404 Not Found if the task is not found.
Conclusion
This project uses Docker Compose for local development, providing an isolated and consistent environment for testing and development. For further customization or deployment to production, additional configurations may be required.