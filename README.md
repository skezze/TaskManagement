# Task Management System

## Overview

The Task Management System is a comprehensive solution for managing tasks and user authentication. It includes a set of APIs for user registration, login, task creation, updates, and retrieval.

## Architecture

### Design Choices

1. **Architecture**: The system is designed using a layered architecture with separation of concerns. It includes:
   - **Controllers**: Handle HTTP requests and responses.
   - **Services**: Contain business logic.
   - **Repositories**: Interface with the database.
   - **DTOs**: Data Transfer Objects for data encapsulation and validation.

2. **Authentication**: Uses JWT (JSON Web Tokens) for secure user authentication and authorization.

3. **Database**: PostgreSQL is used as the database for storing user and task information.

4. **Logging**: Serilog is used for logging, configured to write logs to files for better traceability.

5. **Deployment**: Docker Compose is used for local deployment, ensuring a consistent environment.

## Setup Instructions

### Prerequisites

- Docker (for running Docker containers)
- Docker Compose (for managing multi-container Docker applications)

### Running the Project Locally

1. **Clone the Repository**:
- git clone https://github.com/your-repo/task-management-system.git 
- cd task-management-system

2. **Build and Start Containers**:
-docker-compose up --build

3. **Access the Application**:
- The API will be available at `https://localhost:5000` or swagger at `https://localhost:5000/swagger/index.html`.

4. **Stopping the Application**:
To stop the application, press `CTRL+C` or run:
- docker-compose down

## API Documentation

### User Endpoints

- **Register User**
- **Endpoint**: `POST /users/register`
- **Request Body**:
 ```json
 {
   "username": "string",
   "password": "string",
   "email": "string"
 }
 ```
- **Response**: `201 Created` on success, `400 Bad Request` on error.

- **Login User**
- **Endpoint**: `POST /users/login`
- **Request Body**:
 ```json
 {
   "usernameOrEmail": "string",
   "password": "string"
 }
 ```
- **Response**: `200 OK` with JWT token on success, `401 Unauthorized` on error.

### Task Endpoints

- **Create Task**
- **Endpoint**: `POST /tasks`
- **Request Body**:
 ```json
 {
   "title": "string",
   "description": "string",
   "dueDate": "2024-01-01T00:00:00Z",
   "status": "Pending",
   "priority": "High"
 }
 ```
- **Response**: `201 Created` on success, `400 Bad Request` on error.

- **Get Tasks**
- **Endpoint**: `GET /tasks`
- **Query Parameters**: `status`, `dueDate`, `priority`, `sortBy`, `descending`, `pageNumber`, `pageSize`
- **Response**: `200 OK` with a list of tasks.

- **Get Task by ID**
- **Endpoint**: `GET /tasks/{id}`
- **Response**: `200 OK` with task details, `404 Not Found` if the task is not found.

- **Update Task**
- **Endpoint**: `PUT /tasks/{id}`
- **Request Body**:
 ```json
 {
   "title": "string",
   "description": "string",
   "dueDate": "2024-01-01T00:00:00Z",
   "status": "Pending",
   "priority": "High"
 }
 ```
- **Response**: `200 OK` on success, `400 Bad Request` on error.

- **Delete Task**
- **Endpoint**: `DELETE /tasks/{id}`
- **Response**: `204 No Content` on success, `404 Not Found` if the task is not found.

## Conclusion

This project uses Docker Compose for local development, providing an isolated and consistent environment for testing and development. For further customization or deployment to production, additional configurations may be required.

For more details, please refer to the documentation or contact the development team.
