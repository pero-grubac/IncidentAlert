<div align="center">

# 🚨 Incident Alert

![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-Web_API-512BD4?logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-.NET_8-239120?logo=csharp&logoColor=white)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-Database-4169E1?logo=postgresql&logoColor=white)
![RabbitMQ](https://img.shields.io/badge/RabbitMQ-Messaging-FF6600?logo=rabbitmq&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-Containerized-2496ED?logo=docker&logoColor=white)
![Entity Framework](https://img.shields.io/badge/Entity_Framework-Core-512BD4?logo=dotnet&logoColor=white)

</div>

---

## 📌 Project Overview

**Incident Alert** is a microservices-based system for incident reporting and management. Four independent services communicate via RabbitMQ to handle incident submissions, moderation, statistics, and ML-based grouping.

---

## 🛠️ Services

### 📣 Incident Alert
REST API for general users — create and view incidents. When an incident is created, it is published via RabbitMQ using MassTransit to the Management service.

### 👩‍⚖️ Incident Alert Management
REST API for moderators — review, approve, reject, or delete incident submissions received from the main service.

### 📊 Incident Alert Statistics
Fetches all incidents and generates statistical insights including the location with the most incidents overall, by year, month, and day — both globally and per category.

### 🤖 Incident Alert ML
Retrieves all incidents and groups them by text similarity to identify related or duplicate reports.

---

## 🛠️ Tech Stack

| Technology | Usage |
|------------|-------|
| ASP.NET Core 8 | REST API framework |
| Entity Framework Core | ORM and database migrations |
| PostgreSQL | Relational database (one per service) |
| RabbitMQ + MassTransit | Event-driven messaging between services |
| Docker + Docker Compose | Full containerization |
| JWT | Authentication for Management service |
| Serilog | Structured logging |

---

## 🚀 Setup & Run

### Prerequisites
- Docker and Docker Compose
- .NET 8 SDK (for local development only)

### 1. Clone the repository
```bash
git clone https://github.com/pero-grubac/IncidentAlert.git
cd IncidentAlert
```

### 2. Configure connection strings

Copy and update `appsettings.json` for each service with your database credentials if needed. Default values work with the provided Docker Compose setup.

### 3. Start all services
```bash
docker compose up -d --build
```

Services will be available at:

| Service | Port |
|---------|------|
| Incident Alert API | http://localhost:5000 |
| Incident Alert Management API | http://localhost:5002 |
| Incident Alert ML API | http://localhost:5004 |
| Incident Alert Statistics API | http://localhost:5006 |

### Frontend repositories
- [IncidentAlertFrontend](https://github.com/pero-grubac/IncidentAlertFrontend) — User application
- [IncidentAlertFrontend-Management](https://github.com/pero-grubac/IncidentAlertFrontend-Mangement) — Moderator application

---

## 🏗️ Architecture

```
User Frontend ──── Incident Alert API ──── PostgreSQL
                          │
                       RabbitMQ
                          │
              ┌───────────┼───────────┐
              │           │           │
         Management    Statistics    ML
            API           API        API
              │
          PostgreSQL
```