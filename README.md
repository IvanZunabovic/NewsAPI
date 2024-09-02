# 📰 News API with YARP Reverse Proxy

This project implements a news-related web and mobile application backend using .NET 8, following Clean Architecture principles. The solution is built with a focus on maintainability, scalability, and clean code practices.

## 📐 Architecture

### Clean Architecture
The application is structured following Clean Architecture, promoting a clear separation of concerns. The solution is divided into core layers:

#### Domain: 
Contains the core business logic and entities.
#### Application: 
Handles all business rules using CQRS (Command Query Responsibility Segregation) with the MediatR library.
#### Infrastructure: 
Includes database access (EF Core), external services
#### API: 
Exposes the application’s functionalities as REST endpoints.
#### YARP Reverse Proxy: 
Utilizes YARP to route API requests efficiently, with RoundRobin load balancing for distributing traffic across multiple API instances.

### CQRS with MediatR
The project implements CQRS with the MediatR pattern. This pattern separates read and write operations, improving performance, scalability, and maintainability. MediatR is used to handle commands and queries in a decoupled and testable manner.

### Result Pattern
To improve error handling and ensure consistent responses, the Result pattern is utilized throughout the application. This allows for clear and descriptive error messages, reducing ambiguity and making the API more user-friendly.

## 🚀 Features

Basic Signup/Login: Secure user authentication
News Articles Management: Allows authors to create, edit, and manage news articles.
Author Profiles: Manages author information.

## 🛠️ Technologies Used

.NET 8
Entity Framework Core for database management
MediatR for CQRS pattern implementation
YARP (Yet Another Reverse Proxy) for reverse proxy setup
Docker & Docker Compose for containerization and environment orchestration
SQL Server as the database

## 📦 Setup

Clone the Repository

```bash

git clone https://github.com/your-username/your-repo-name.git
cd your-repo-name

```

Build and Run with Docker Compose

```bash

docker-compose up -d --build

```
Access the API 

The API definition will be available at http://localhost:4000/swagger/index.html using swagger UI.

## 🤝 Contributing
Contributions are welcome! Please fork the repository and submit a pull request.
