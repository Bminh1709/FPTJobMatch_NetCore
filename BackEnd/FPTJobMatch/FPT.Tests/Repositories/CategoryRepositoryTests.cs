using FPT.DataAccess.Data;
using FPT.DataAccess.Repository.IRepository;
using FPT.DataAccess.Repository;
using FPT.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;

namespace FPT.Tests.Repositories
{
    public class CategoryRepositoryTests : IDisposable
    {
        private ApplicationDbContext _context;
        private ICategoryRepository _categoryRepository;

        public CategoryRepositoryTests()
        {
            _context = GetDbContext().Result;
            _categoryRepository = new CategoryRepository(_context);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        private async Task<ApplicationDbContext> GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);

            // Seed some categories for testing
            context.Categories.AddRange(new List<Category>
            {
                new Category { Id = 1, Name = "Category 1", IsApproved = true, CreatedAt = DateTime.UtcNow, CreatedByUserId = "user1" },
                new Category { Id = 2, Name = "Category 2", IsApproved = false, CreatedAt = DateTime.UtcNow, CreatedByUserId = "user2" },
                new Category { Id = 3, Name = "Category 3", IsApproved = true, CreatedAt = DateTime.UtcNow, CreatedByUserId = "user3" }
            });

            await context.SaveChangesAsync();

            return context;
        }

        [Fact]
        public async Task GetCategoriesByStatus_ReturnsCorrectCategories()
        {
            // Act
            var approvedCategories = await _categoryRepository.GetCategoriesByStatus(isApproved: true);
            var unapprovedCategories = await _categoryRepository.GetCategoriesByStatus(isApproved: false);

            // Assert
            approvedCategories.Should().HaveCount(2);
            unapprovedCategories.Should().HaveCount(1);
        }

        [Fact]
        public void CountCategories_ReturnsCorrectCount()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Category 1", CreatedAt = DateTime.UtcNow },
                new Category { Id = 2, Name = "Category 2", CreatedAt = DateTime.UtcNow },
                new Category { Id = 3, Name = "Category 3", CreatedAt = DateTime.UtcNow }
            };

            // Act
            var countThisMonth = _categoryRepository.CountCategories(categories, isThisMonth: true);
            var countAll = _categoryRepository.CountCategories(categories, isThisMonth: false);

            // Assert
            countThisMonth.Should().Be(3); // Assuming all categories are created this month
            countAll.Should().Be(3);
        }

        [Fact]
        public async Task NullifyCreatedByUserIdAsync_NullifiesUserId()
        {
            // Arrange
            var userId = "user1";

            // Act
            await _categoryRepository.NullifyCreatedByUserIdAsync(userId);
            await _context.SaveChangesAsync();

            // Assert
            var categories = await _context.Categories.Where(c => c.CreatedByUserId == userId).ToListAsync();
            categories.Should().BeEmpty();
        }
    }
}
