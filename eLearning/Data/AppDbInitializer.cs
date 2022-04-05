using eLearning.Data.Static;
using eLearning.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eLearning.Data
{
    public class AppDbInitializer
    {
        public static async Task SeedUsersAndRolesAsync(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                //Roles
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));

                if (!await roleManager.RoleExistsAsync(UserRoles.User))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.User));


                //Users
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                var admin = await userManager.FindByEmailAsync("admin@admin.com");
                if(admin == null)
                {
                    var newAdminUser = new ApplicationUser()
                    {
                        FullName = "Admin Admin",
                        UserName = "Admin",
                        Email = "admin@admin.com",
                        EmailConfirmed = true,
                    };
                    await userManager.CreateAsync(newAdminUser, "Admin123!");
                    await userManager.AddToRoleAsync(newAdminUser, UserRoles.Admin);
                }

                var user = await userManager.FindByEmailAsync("user@user.com");
                if (user == null)
                {
                    var newUser = new ApplicationUser()
                    {
                        FullName = "User User",
                        UserName = "User",
                        Email = "user@user.com",
                        EmailConfirmed = true,
                    };
                    await userManager.CreateAsync(newUser, "User123!");
                    await userManager.AddToRoleAsync(newUser, UserRoles.User);
                }
            }
        }
    }
}
