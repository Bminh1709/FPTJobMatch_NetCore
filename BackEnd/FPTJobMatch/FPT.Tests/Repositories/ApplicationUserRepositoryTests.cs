using FluentAssertions;
using FPT.DataAccess.Data;
using FPT.DataAccess.Repository;
using FPT.Models;
using FPT.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FPT.Tests.Repositories
{
    public class ApplicationUserRepositoryTests : IDisposable
    {
        private ApplicationDbContext _context;

        public ApplicationUserRepositoryTests()
        {
            _context = GetDbContext().Result;
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        private async Task<ApplicationDbContext> GetDbContext(bool seedTestData = true)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);

            if (seedTestData)
            {
                SeedTestData(context);
            }

            return context;
        }

        private void SeedTestData(ApplicationDbContext context)
        {
            // Seed roles
            context.Roles.Add(new IdentityRole { Id = "role1", Name = SD.Role_JobSeeker });
            context.Roles.Add(new IdentityRole { Id = "role2", Name = SD.Role_Employer });
            context.Roles.Add(new IdentityRole { Id = "role3", Name = SD.Role_Admin });

            // Seed users
            for (int i = 1; i <= 15; i++)
            {
                context.Users.Add(new ApplicationUser { Id = $"user{i}", Name = $"Test User {i}", Email = $"test{i}@example.com", CreatedAt = DateTime.Now, AccountStatus = SD.StatusActive });
            }

            // Assign roles to users
            context.UserRoles.Add(new IdentityUserRole<string> { UserId = "user1", RoleId = "role1" });
            context.UserRoles.Add(new IdentityUserRole<string> { UserId = "user2", RoleId = "role2" });

            context.SaveChanges();
        }

        private async Task<(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ApplicationUserRepository repository)> SetupTest(bool seedTestData = true)
        {
            var context = await GetDbContext(seedTestData);
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context), null, null, null, null, null, null, null, null);
            var repository = new ApplicationUserRepository(context, userManager);
            return (context, userManager, repository);
        }

        [Fact]
        public async Task GetFilteredUsersAsync_ReturnsCorrectUsers()
        {
            // Arrange
            var (context, userManager, repository) = await SetupTest();

            // Act
            var result = await repository.GetFilteredUsersAsync(userType: null, sortType: "NewestFirst", keyword: "Test", pageIndex: 1);

            // Assert
            result.Should().HaveCount(10);
        }

        [Fact]
        public async Task GetFilteredUsersAsync_ReturnsEmptyList_WhenNoKeywordMatches()
        {
            // Arrange
            var (context, userManager, repository) = await SetupTest();

            // Act
            var result = await repository.GetFilteredUsersAsync(userType: null, sortType: "NewestFirst", keyword: "nonexistent", pageIndex: 1);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetFilteredUsersAsync_ReturnsCorrectPage()
        {
            // Arrange
            var (context, userManager, repository) = await SetupTest();

            // Act
            var result = await repository.GetFilteredUsersAsync(userType: null, sortType: null, keyword: null, pageIndex: 2);

            // Assert
            result.Should().HaveCount(5);
        }
    }

}
