using FPT.DataAccess.Data;
using FPT.Models;
using FPT.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FPT.DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;

        public DbInitializer(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext db)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _db = db;
        }

        public void Initialize()
        {
            // Migrations if they are not applied
            try
            {
                // Check if there are any pending migrations
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    // Apply pending migrations to update the database schema
                    _db.Database.Migrate();
                }
            }
            catch (Exception ex) { }

            if (!_roleManager.RoleExistsAsync(SD.Role_JobSeeker).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Role_JobSeeker)).GetAwaiter().GetResult();
            }
            if (!_roleManager.RoleExistsAsync(SD.Role_Employer).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Employer)).GetAwaiter().GetResult();
            }
            if (!_roleManager.RoleExistsAsync(SD.Role_Admin).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
            }

            if (_userManager.FindByEmailAsync("minhbee203@gmail.com").Result == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = "minhbee203@gmail.com",
                    Email = "minhbee203@gmail.com",
                    Name = "Bui Minh",
                    CreatedAt = DateTime.UtcNow,
                    AccountStatus = SD.StatusActive,
                    EmailConfirmed = true
                };

                var result = _userManager.CreateAsync(adminUser, "Aa12345.").Result;

                if (result.Succeeded)
                {
                    // Add roles to the admin user
                    _userManager.AddToRolesAsync(adminUser, new[] { SD.Role_Admin }).Wait();
                }
            }
        }
    }
}
