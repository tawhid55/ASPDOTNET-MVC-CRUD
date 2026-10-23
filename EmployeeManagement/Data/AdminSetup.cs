using Microsoft.AspNetCore.Identity;
namespace EmployeeManagement.Data;

public static class AdminSetup
{
    public static async Task CreateAsync(IServiceProvider services, IConfiguration configuration)
    {
        var email = configuration["Admin:Email"]?.Trim();
        var password = configuration["Admin:Password"];
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            throw new InvalidOperationException("Set Admin:Email and Admin:Password before running --create-admin.");
        var roles = services.GetRequiredService<RoleManager<IdentityRole>>();
        var users = services.GetRequiredService<UserManager<IdentityUser>>();
        if (!await roles.RoleExistsAsync("Admin"))
            Check(await roles.CreateAsync(new IdentityRole("Admin")));
        if (await users.FindByEmailAsync(email) != null)
            throw new InvalidOperationException("This email already exists. Use a new email for administrator setup.");
        var user = new IdentityUser { UserName = email, Email = email };
        Check(await users.CreateAsync(user, password));
        Check(await users.AddToRoleAsync(user, "Admin"));
        Console.WriteLine("Administrator created.");
    }
    private static void Check(IdentityResult result)
    {
        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join(" ", result.Errors.Select(error => error.Description)));
    }
}
