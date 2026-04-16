# 🚀 Full-Forum  
A fullstack forum application built with **.NET 8**, designed to reflect real-world backend architecture and modern web development practices.

---

## 📸 Preview

![API Swagger](https://github.com/user-attachments/assets/f91f652b-9c99-4f72-8929-757e5ffcd1e4)  
![Login UI](https://github.com/user-attachments/assets/a2b0c207-f2ea-4a9b-8e85-60f1c2249c3c)

---

## 🧠 Overview  
Full-Forum is a structured fullstack application where users can register, authenticate, and interact through threads and comments.

The project focuses on building a **scalable backend architecture** with clean separation of concerns, while integrating a functional UI using Blazor Server.

---

## 🛠 Tech Stack  

**Backend**
- .NET 8  
- ASP.NET Core Minimal API  
- Entity Framework Core  
- SQLite  
- ASP.NET Core Identity  
- JWT Authentication  

**Frontend**
- Blazor Server  

---

## ✨ Core Features  

- 🔐 Authentication with **JWT + Identity**
- 👥 Role-based authorization (**Admin / User**)  
- 🧵 Threads, categories, and comments  
- 💬 Threaded comments using `ParentCommentId`  
- 🗑 Soft delete functionality  
- ⚠️ Structured error handling with `ProblemDetails`  
- 📦 Clean and modular API structure  

---

## 🏗 Architecture  

The project follows a Clean Architecture-inspired structure:

- **Domain**  
  Core entities and business rules  

- **Application**  
  Business logic and use cases  

- **Infrastructure**  
  Database, Identity, and external services  

- **WebApi**  
  API endpoints and authentication  

- **WebUi**  
  Blazor frontend  

---

## 🎯 Key Focus Areas  

- Building a backend similar to real-world systems  
- Implementing authentication and authorization  
- Designing clear and maintainable API endpoints  
- Handling data safely with validation and soft delete  
- Separating concerns for scalability  

---

## 🚧 Status  

Ongoing project — continuously improving and adding features.

---

## 🧪 How to Run  

```bash
# Start API
dotnet run --project src/ABFM-Forum.WebApi

# Start UI (optional)
dotnet run --project src/ABFM-Forum.WebUi



