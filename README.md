# Employee Management

A simple ASP.NET Core MVC application using .NET 8, SQL Server, EF Core, ASP.NET Core Identity, and Bootstrap.

## Permissions

- Guests can register and log in.
- Registered users can view the dashboard, employee list, and employee details.
- Administrators can also add, edit, and delete employees.
- Registration never grants administrator access.

Passwords are hashed by Identity. All form submissions have CSRF protection. Authentication cookies require HTTPS. Five failed login attempts lock an account for 15 minutes.

## Run locally

1. Set `ConnectionStrings:EmployeeConnectionString` in `EmployeeManagement/appsettings.Development.json` for your SQL Server.
2. Apply the migrations:

   ```powershell
   dotnet ef database update --project EmployeeManagement -- --environment Development
   ```

3. Create an administrator using a new email address:

   ```powershell
   ./scripts/create-admin.ps1
   ```

   Enter the email and password when prompted. Use at least 8 characters with uppercase, lowercase, a number, and a symbol. Credentials are passed through temporary process environment variables, never saved to the repository. Existing accounts are not silently promoted or overwritten.

4. Start the app:

   ```powershell
   dotnet run --project EmployeeManagement
   ```

5. Open https://localhost:7151 and log in. Use Register for regular, view-only accounts.

Stop the running app before rebuilding so compiled Razor views and static styles stay in sync. For deployment, apply migrations before starting the app and configure the production SQL Server connection and HTTPS certificate.

## Routes

| Route | Access |
| --- | --- |
| `/Account/Register`, `/Account/Login` | Public |
| `/Account/Logout` | Signed in, POST only |
| `/`, `/Employees`, `/Employees/Details/{id}` | Signed in |
| `/Employees/Add`, `/Employees/Edit/{id}` | Admin |
| `/Employees/Delete` | Admin, POST only |

Email confirmation and password recovery are not configured; no email provider is required for this local application.

## Integration checks

The tests use the separate `MvcCrudAuthTest` database on `.\SQLEXPRESS` and HTTPS port 7255. They create unique test accounts, create and delete a test employee, and leave existing application records untouched. Test accounts remain in the test database for inspection.

```powershell
dotnet build EmployeeManagement
dotnet ef database update --project EmployeeManagement --connection 'Server=.\SQLEXPRESS;Database=MvcCrudAuthTest;Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True' -- --environment Development
node tests/auth-crud.cjs
```

Checks cover registration, login/logout, role restrictions on GET and POST, attempted role injection, CSRF rejection, safe return URLs, failed-login lockout, duplicate registration, validation, details, create, update, delete, and missing records.

Identity configuration follows [Microsoft's ASP.NET Core Identity documentation](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity-configuration?view=aspnetcore-8.0).
