using FPT.DataAccess.Data;
using FPT.DataAccess.Repository.IRepository;
using FPT.Models;
using FPT.Utility;
using FPT.Utility.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace FPT.DataAccess.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public ApplicationUserRepository(ApplicationDbContext db, UserManager<ApplicationUser> userManager) : base(db)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<PaginatedList<ApplicationUser>> GetFilteredUsersAsync(string? userType, string? sortType, string? keyword, int pageIndex)
        {
            // Fetch the role ID associated with the "Admin" role
            var adminRoleId = await _db.Roles
                .Where(r => r.Name == SD.Role_Admin)
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            // Fetch users excluding those with the Admin role
            IQueryable<ApplicationUser> users = _db.ApplicationUsers
                .Where(u => !_db.UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId == adminRoleId));


            // Filter users based on userType
            if (!string.IsNullOrEmpty(userType))
            {
                var role = userType switch
                {
                    SD.Role_JobSeeker => SD.Role_JobSeeker,
                    SD.Role_Employer => SD.Role_Employer,
                    _ => null
                };

                if (role != null)
                {
                    // Fetch the role ID associated with the specified role
                    var roleId = await _db.Roles
                        .Where(r => r.Name == role)
                        .Select(r => r.Id)
                        .FirstOrDefaultAsync();

                    // Filter users with the specified role
                    users = users.Where(u => _db.UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId == roleId));
                }
            }

            // Exclude Admin
            // users = users.Where(u => !_userManager.IsInRoleAsync(u, SD.Role_Admin).Result);
            // => Error: Could not be translated
            // => Reason: Entity Framework Core is unable to translate the asynchronous operation IsInRoleAsync into SQL

            // Sort users based on sortType
            if (!string.IsNullOrEmpty(sortType))
            {
                if (sortType == "NewestFirst")
                {
                    users = users.OrderByDescending(u => u.CreatedAt);
                }
                else if (sortType == "OldestFirst")
                {
                    users = users.OrderBy(u => u.CreatedAt);
                }
            }

            // Filter users based on keyword
            if (!string.IsNullOrEmpty(keyword))
            {
                users = users.Where(u => u.Name.ToLower().Contains(keyword.ToLower()) || u.Email.ToLower().Contains(keyword.ToLower()));
            }

            // Set Page Size
            int pageSize = 10;

            return await PaginatedList<ApplicationUser>.CreateAsync(users, pageIndex, pageSize);
        }

        public void Update(ApplicationUser applicationUser)
        {
            _db.ApplicationUsers.Update(applicationUser);
        }
    }
}
