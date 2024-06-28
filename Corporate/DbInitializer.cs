using Microsoft.AspNetCore.Identity;
using Corporate.Models;
using System.Linq;
using System;

namespace Corporate
{
    public class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // Ensuring the database is created
            context.Database.EnsureCreated();

            if (!context.Product.Any())
            {
                context.Product.AddRange(
                    new Product { Name = "HDD 1TB", Quantity = 55, Price = 74.09m },
                    new Product { Name = "HDD SSD 512GB", Quantity = 102, Price = 190.99m },
                    new Product { Name = "RAM DDR4 16GB", Quantity = 47, Price = 80.32m }
                );
                context.SaveChanges();
            }
        }

        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
                var userManager = serviceScope.ServiceProvider.GetService<UserManager<ApplicationUser>>();
                var roleManager = serviceScope.ServiceProvider.GetService<RoleManager<IdentityRole>>();

                // Ensures roles exist
                string[] roleNames = { "Admin", "User" };
                foreach (var roleName in roleNames)
                {
                    var roleExists = roleManager.RoleExistsAsync(roleName).Result;
                    if (!roleExists)
                    {
                        var roleResult = roleManager.CreateAsync(new IdentityRole(roleName)).Result;
                        if (!roleResult.Succeeded)
                        {
                            throw new ApplicationException($"Error creating role '{roleName}'.");
                        }
                    }
                }

                // Ensures admin user exists
                var adminUser = userManager.FindByEmailAsync("admin@example.com").Result;
                if (adminUser == null)
                {
                    adminUser = new ApplicationUser
                    {
                        UserName = "admin@example.com",
                        Email = "admin@example.com",
                        // Add additional properties as needed
                    };
                    var createUserResult = userManager.CreateAsync(adminUser, "Admin@123").Result;
                    if (!createUserResult.Succeeded)
                    {
                        throw new ApplicationException($"Error creating admin user: {createUserResult.Errors}");
                    }
                }

                // Assigning admin role to admin user if not already assigned
                var rolesForAdmin = userManager.GetRolesAsync(adminUser).Result;
                if (!rolesForAdmin.Contains("Admin"))
                {
                    var addRoleResult = userManager.AddToRoleAsync(adminUser, "Admin").Result;
                    if (!addRoleResult.Succeeded)
                    {
                        throw new ApplicationException($"Error assigning admin role to admin user: {addRoleResult.Errors}");
                    }
                }

                // Ensures user user exists
                var normalUser = userManager.FindByEmailAsync("user@example.com").Result;
                if (normalUser == null)
                {
                    normalUser = new ApplicationUser
                    {
                        UserName = "user@example.com",
                        Email = "user@example.com",
                    };
                    var createUserResult = userManager.CreateAsync(normalUser, "User@123").Result;
                    if (!createUserResult.Succeeded)
                    {
                        throw new ApplicationException($"Error creating normal user: {createUserResult.Errors}");
                    }
                }

                // Assigning user role to normal user if not already assigned
                var rolesForNormalUser = userManager.GetRolesAsync(normalUser).Result;
                if (!rolesForNormalUser.Contains("User"))
                {
                    var addRoleResult = userManager.AddToRoleAsync(normalUser, "User").Result;
                    if (!addRoleResult.Succeeded)
                    {
                        throw new ApplicationException($"Error assigning user role to normal user: {addRoleResult.Errors}");
                    }
                }
            }
        }
    }
}
