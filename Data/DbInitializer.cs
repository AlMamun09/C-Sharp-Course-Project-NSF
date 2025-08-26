using Microsoft.AspNetCore.Identity;
using LocalScout.Data;
using System.Threading.Tasks;

namespace LocalScout.Data
{
    public class DbInitializer
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager; // Add this

        // Updated constructor
        public DbInitializer(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager; // Add this
        }

        public async Task SeedAsync()
        {
            // Seed Roles (Admin, ServiceProvider, User)
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            if (!await _roleManager.RoleExistsAsync("ServiceProvider"))
            {
                await _roleManager.CreateAsync(new IdentityRole("ServiceProvider"));
            }
            if (!await _roleManager.RoleExistsAsync("User"))
            {
                await _roleManager.CreateAsync(new IdentityRole("User"));
            }

            // Seed a Default Admin User
            if (await _userManager.FindByEmailAsync("abd.al.mamun001@gmail.com") == null)
            {
                var user = new ApplicationUser
                {
                    UserName = "abd.al.mamun001@gmail.com",
                    Email = "abd.al.mamun001@gmail.com",
                    FirstName = "Admin",
                    LastName = "User",
                    Address = "College Gate, Tongi",
                    EmailConfirmed = true // Bypass email confirmation for the admin
                };

                // Create the user with a password
                var result = await _userManager.CreateAsync(user, "@lMamun#2001$");

                if (result.Succeeded)
                {
                    // Assign the 'Admin' role to the new user
                    await _userManager.AddToRoleAsync(user, "Admin");
                }
            }
        }
    }
}