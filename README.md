# Full-Forum

A fullstack forum application built with .NET 8 where users can register, log in, create threads and comments.

## 📸 API Overview (Swagger)

<img width="773" height="878" alt="API Swagger" src="https://github.com/user-attachments/assets/f91f652b-9c99-4f72-8929-757e5ffcd1e4" />

## Overview
This project is built to simulate a real-world system with a clear architecture and focus on backend structure, authentication and data handling.

## 🛠 Tech Stack
- .NET 8
- ASP.NET Core Minimal API
- Entity Framework Core
- SQLite
- ASP.NET Core Identity
- JWT Authentication
- Blazor Server (UI)

## Features
- User authentication (JWT + Identity)
- Role-based access (Admin/User)
- Categories, threads and comments
- Threaded comments (ParentCommentId)
- Soft delete functionality
- Structured API with ProblemDetails error handling

## Architecture
The project follows a clean separation of concerns:

- **Domain** – core entities and rules  
- **Application** – business logic and use cases  
- **Infrastructure** – database and external services  
- **WebApi** – API endpoints and authentication  
- **WebUi** – Blazor frontend  

## What I focused on
- Building a structured backend similar to real systems  
- Implementing authentication and authorization  
- Designing clear API endpoints  
- Handling data safely with validation and soft delete  

## Status
🚧 Ongoing project – continuously improving and adding features  

## How to run

1. Start API:
```bash
dotnet run --project src/ABFM-Forum.WebApi




