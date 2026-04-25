# 🛒 Talabat - E-Commerce REST API

A production-ready **RESTful API** for an e-commerce platform inspired by Talabat, built with **ASP.NET Core (.NET 8)** following **Clean Architecture** principles.

---

## 📋 Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Tech Stack](#tech-stack)
- [Architecture](#architecture)
- [API Endpoints](#api-endpoints)
- [Database Schema](#database-schema)
- [Getting Started](#getting-started)
- [Project Structure](#project-structure)

---

## 📌 Overview

Talabat is a backend REST API that powers a full e-commerce experience. It handles product browsing with advanced filtering and pagination, shopping basket management via Redis, order processing with multiple delivery methods, secure JWT-based authentication, and real-time payment processing through Stripe — all documented via Swagger.

---

## ✨ Features

### 🛍️ Products
- Browse all products with **server-side filtering**, **sorting**, and **pagination**
- Filter by brand and category
- Get product details with brand and category info

### 🧺 Shopping Basket
- Create and update shopping baskets stored in **Redis** (in-memory cache)
- Retrieve and delete baskets by ID

### 📦 Orders
- Place orders linked to authenticated users
- Support multiple **delivery methods** with different costs
- Track order status (Pending, PaymentReceived, PaymentFailed, etc.)
- View all user orders and specific order details

### 💳 Payments (Stripe Integration)
- Create or update **Stripe Payment Intents** for orders
- Handle **Stripe Webhooks** to automatically update order status on payment success/failure

### 🔐 Authentication & Authorization
- User **Register** and **Login** with JWT tokens
- Get and update authenticated user address
- Role-based access control using **ASP.NET Core Identity**
- JWT Bearer token authentication with configurable issuer, audience, and expiry

### 📄 API Documentation
- Full **Swagger / OpenAPI** documentation for all endpoints
- Standardized API error responses with custom `ApiResponse` wrapper

### 🛡️ Error Handling
- Global exception handling via custom **ExceptionMiddleware**
- Standardized error responses (400, 401, 404, 500)
- Validation error responses via `ApiValidationErrorResponse`

---

## 🛠️ Tech Stack

| Layer | Technology |
|-------|------------|
| Framework | ASP.NET Core Web API (.NET 8) |
| Language | C# |
| ORM | Entity Framework Core 8 |
| Database (Business) | Microsoft SQL Server |
| Database (Identity) | Microsoft SQL Server (separate DB) |
| Cache / Basket | Redis (StackExchange.Redis) |
| Authentication | ASP.NET Core Identity + JWT Bearer |
| Payments | Stripe API |
| Object Mapping | AutoMapper 12 |
| API Docs | Swagger / Swashbuckle |

---

## 🏛️ Architecture

The solution follows **Clean Architecture** (Onion Architecture) separated into 4 projects:

```
Talabat Solution/
│
├── Talabat.Core/                  # Domain Layer (innermost)
│   ├── Entities/                  # Domain models (Product, Order, Basket...)
│   │   ├── Identity/              # AppUser, Address (Identity entities)
│   │   └── Order/                 # Order, OrderItem, DeliveryMethod...
│   ├── Repositories.Contract/     # Repository interfaces
│   ├── Services.Interfaces/       # Service interfaces (IOrderService, IPaymentService...)
│   ├── Specifications.Contract/   # Specification interfaces & base class
│   └── IUnitOfWork.cs
│
├── Talabat.Repository/            # Infrastructure / Data Layer
│   ├── Data/
│   │   ├── StoreDbContext.cs      # Main EF Core DbContext
│   │   ├── Configurations/        # Fluent API entity configurations
│   │   ├── DataSeed/              # JSON seed data (products, brands, categories)
│   │   └── Migrations/            # EF Core migrations (Store)
│   ├── Identity/
│   │   ├── AppIdentityDbContext   # Identity DbContext (separate DB)
│   │   ├── AppIdentityDbContextSeed  # Default user seeding
│   │   └── Migrations/            # EF Core migrations (Identity)
│   ├── Repositories/
│   │   ├── GenaricRepository.cs   # Generic repository with Specification support
│   │   └── BasketRepository.cs    # Redis-based basket repository
│   ├── Specifications/
│   │   └── SpecificationsEvaluator.cs
│   └── UnitOfWork.cs
│
├── Talabat.Services/              # Business Logic Layer
│   ├── OrderService.cs            # Order creation and management
│   ├── PaymentService.cs          # Stripe payment intent management
│   └── TokenService.cs            # JWT token generation
│
└── Talabat.APIs/                  # Presentation Layer (outermost)
    ├── Controllers/               # API Controllers
    ├── Dtos/                      # Data Transfer Objects
    ├── Errors/                    # Custom API error responses
    ├── Extensions/                # Service registration extensions
    ├── Helpers/                   # AutoMapper profiles, Pagination, URL resolvers
    ├── Middlewares/               # Global exception middleware
    └── Program.cs                 # Entry point & DI configuration
```

### Design Patterns Used
- **Repository Pattern** — Generic repository with Specification support
- **Specification Pattern** — Encapsulates query logic (filtering, sorting, includes, pagination)
- **Unit of Work Pattern** — Coordinates multiple repository transactions
- **Dependency Injection** — All services registered via built-in DI container
- **CQRS-inspired DTOs** — Separate input/output models for clean API contracts

---

## 🌐 API Endpoints

### Products — `/api/Products`
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/Products` | Get all products (filter, sort, paginate) |
| GET | `/api/Products/{id}` | Get product by ID |
| GET | `/api/Products/brands` | Get all brands |
| GET | `/api/Products/types` | Get all categories |

### Basket — `/api/Basket`
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/Basket` | Get basket by ID |
| POST | `/api/Basket` | Create or update basket |
| DELETE | `/api/Basket` | Delete basket |

### Orders — `/api/Orders` *(Authorized)*
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/Orders` | Create a new order |
| GET | `/api/Orders` | Get all orders for logged-in user |
| GET | `/api/Orders/{id}` | Get specific order by ID |
| GET | `/api/Orders/deliveryMethods` | Get all delivery methods |

### Payments — `/api/Payments` *(Authorized)*
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/Payments/{basketId}` | Create or update payment intent |
| POST | `/api/Payments/webhook` | Stripe webhook handler |

### Accounts — `/api/Accounts`
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/Accounts/register` | Register new user |
| POST | `/api/Accounts/login` | Login and get JWT token |
| GET | `/api/Accounts` | Get current user info *(Authorized)* |
| GET | `/api/Accounts/address` | Get user address *(Authorized)* |
| PUT | `/api/Accounts/address` | Update user address *(Authorized)* |

---

## 🗄️ Database Schema

### Store Database (`TalabatDb.APIs`)
| Entity | Description |
|--------|-------------|
| `Product` | Product with name, price, brand, category, picture |
| `ProductBrand` | Product brand |
| `ProductType` | Product category/type |
| `Order` | Order with buyer email, shipping address, status |
| `OrderItem` | Individual items within an order |
| `DeliveryMethod` | Delivery options with cost |

### Identity Database (`TalabatDb.APIs.Identity`)
| Entity | Description |
|--------|-------------|
| `AppUser` | User with display name and address |
| `Address` | User's saved address |

### Redis
| Key | Description |
|-----|-------------|
| `basket:{id}` | Shopping basket stored as JSON in Redis |

---

## 🚀 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server)
- [Redis](https://redis.io/download) (running on `localhost:6379`)
- [Stripe Account](https://stripe.com) (for payment features)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or VS Code

---

## 📁 Project Structure

### Controllers
| Controller | Responsibility |
|------------|---------------|
| `ProductsController` | Product listing, filtering, brands & types |
| `BasketController` | Shopping basket CRUD via Redis |
| `OrdersController` | Order placement and retrieval |
| `PaymentsController` | Stripe payment intent & webhook |
| `AccountsController` | Register, Login, user profile |
| `BuggyController` | Error testing endpoints |
| `ErrorsController` | Centralized error response handler |

### Services
| Service | Responsibility |
|---------|---------------|
| `OrderService` | Business logic for order creation |
| `PaymentService` | Stripe payment intent creation & webhook handling |
| `TokenService` | JWT token generation |

---

## 📄 License

This project is intended for educational purposes.
