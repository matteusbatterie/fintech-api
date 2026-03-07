# FinTech-API

A high-performance Financial Ledger built with .NET 10, following Clean Architecture and Domain-Driven Design (DDD) principles. The project focuses on strictly encapsulated domain logic and transactional integrity.

## System Requirements
This project was developed and tested using:
* Windows 11
* Visual Studio 2026
* .NET 10.0 SDK
* Docker Desktop (SQL Server 2022 & Redis)

## Getting Started
1. Clone the repository.
2. Create a `.env` file in the root directory.
3. Copy the contents of `.env.example` into `.env` and set your `DB_PASSWORD`.
4. Run `docker-compose up -d` in the root folder.
5. Open `FinTech.sln` in Visual Studio 2026.
