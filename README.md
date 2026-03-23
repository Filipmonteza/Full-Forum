FullForum



FullForum is a .NET 8 forum backend project built using a layered Clean Architecture approach.



The project focuses on designing a scalable and maintainable backend for a discussion platform, with clear separation between domain, application, infrastructure, and API layers.



Project Status



This project is in active development.



Implemented so far:



Domain entities and base abstractions

Application-layer DTOs and use-case handlers (threads and comments)

Repository and service interfaces

Infrastructure setup with Entity Framework Core, ASP.NET Identity, and SQLite



In progress:



Infrastructure persistence implementations

Web API endpoints and composition root

Database seeding and identity bootstrapping



Planned:



Authentication and authorization flows

Pagination and filtering

Validation and error handling improvements

Logging and monitoring

Integration with frontend client (Blazor WebAssembly)

Architecture



The project follows Clean Architecture principles:



Domain layer contains core business logic and entities

Application layer defines use cases and interfaces

Infrastructure layer handles persistence and external concerns

Web API acts as the entry point and composition root



This structure ensures separation of concerns, testability, and scalability.



Tech Stack

.NET 8

ASP.NET Core Web API

Entity Framework Core

SQLite

ASP.NET Core Identity

Solution Structure

FullForum/

&#x20; src/

&#x20;   FullForum-Domain/          # Core entities and domain rules

&#x20;   FullForum-Application/     # Use cases, DTOs, interfaces

&#x20;   FullForum-Infrastructure/  # EF Core, Identity, persistence, seeding

&#x20;   FullForum-WebApi/          # API host (composition root)

