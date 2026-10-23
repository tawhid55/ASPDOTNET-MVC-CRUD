# ASPDOTNET-MVC-CRUD

A simple ASP.NET Core MVC demo application that manages employees with full CRUD
(Create, Read, Update, Delete) functionality.

## Tech stack

- ASP.NET Core MVC on **.NET 8**
- **Entity Framework Core 8** with SQL Server
- Bootstrap 5, jQuery, jQuery Validation
- Razor views

## Features

- Employee list
- Add a new employee
- Edit an existing employee
- Delete an employee (with a confirmation prompt)
- Server-side and client-side validation
- Anti-forgery (CSRF) protection on all POST actions
- Async database calls through EF Core

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (or newer)
- SQL Server Express / LocalDB / full SQL Server

## Setup

1. Set your database connection string in
   `ASPDOTNET-MVC-CRUD/appsettings.Development.json`:

   ```json
   "ConnectionStrings": {
     "DemoConnectionString": "Server=.\\SQLEXPRESS;Database=MvcDemoDb;Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True"
   }
   ```

2. Apply the EF Core migration (creates the database and `Employees` table):

   ```powershell
   dotnet ef database update --project ASPDOTNET-MVC-CRUD
   ```

3. Run the application:

   ```powershell
   dotnet run --project ASPDOTNET-MVC-CRUD
   ```

4. Open one of the URLs from `Properties/launchSettings.json`:

   - https://localhost:7151
   - http://localhost:5136

## Project structure

```
ASPDOTNET-MVC-CRUD/
├── Controllers/
│   ├── EmployeesController.cs   # CRUD actions
│   └── HomeController.cs        # Home / Privacy / Error
├── Data/
│   └── DemoDbContext.cs         # EF Core DbContext
├── Migrations/                  # Initial migration
├── Models/
│   ├── Domain/Employee.cs
│   ├── AddEmployeeViewModel.cs
│   ├── UpdateEmployeeViewModel.cs
│   └── ErrorViewModel.cs
├── Views/
│   ├── Employees/
│   │   ├── Index.cshtml         # Employee list
│   │   ├── Add.cshtml           # Add form
│   │   └── Edit.cshtml          # Edit / delete form
│   ├── Home/
│   └── Shared/
└── wwwroot/                     # Bootstrap, jQuery, CSS, JS
```

## Routes

| Method | Route | Description |
|---|---|---|
| GET | `/Employees` | List all employees |
| GET | `/Employees/Add` | Show the add form |
| POST | `/Employees/Add` | Create an employee |
| GET | `/Employees/Edit/{id}` | Show the edit form |
| POST | `/Employees/Edit` | Update an employee |
| POST | `/Employees/Delete` | Delete an employee |
