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
    public class HelpArticleRepositoryTests : IDisposable
    {
        private ApplicationDbContext _context;
        private IHelpArticleRepository _helpArticleRepository;

        public HelpArticleRepositoryTests()
        {
            _context = GetDbContext().Result;
            _helpArticleRepository = new HelpArticleRepository(_context);
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

            // Seed some help articles for testing
            context.HelpArticles.AddRange(new List<HelpArticle>
            {
                new HelpArticle { Id = 1, Title = "Article 1", Content = "Content 1", UpdatedAt = DateTime.UtcNow, AdminId = "admin1" },
                new HelpArticle { Id = 2, Title = "Article 2", Content = "Content 2", UpdatedAt = DateTime.UtcNow, AdminId = "admin1" },
                new HelpArticle { Id = 3, Title = "Article 3", Content = "Content 3", UpdatedAt = DateTime.UtcNow, AdminId = "admin1" }
            });

            await context.SaveChangesAsync();

            return context;
        }

        [Fact]
        public async Task GetAllArticlesFilteredAsync_ReturnsCorrectArticles_WhenSortTypeAndKeywordProvided()
        {
            // Act
            var articles = await _helpArticleRepository.GetAllArticlesFilteredAsync(sortType: "NewestFirst", keyword: "Article");

            // Assert
            articles.Should().HaveCount(3); // Assuming all articles have "Article" in their title
            articles.Should().BeInDescendingOrder(a => a.UpdatedAt);
        }

        [Fact]
        public async Task GetAllArticlesFilteredAsync_ReturnsCorrectArticles_WhenSortTypeOnlyProvided()
        {
            // Act
            var articles = await _helpArticleRepository.GetAllArticlesFilteredAsync(sortType: "OldestFirst");

            // Assert
            articles.Should().HaveCount(3); // Assuming all articles have "Article" in their title
            articles.Should().BeInAscendingOrder(a => a.UpdatedAt);
        }

        [Fact]
        public async Task GetAllArticlesFilteredAsync_ReturnsCorrectArticles_WhenKeywordOnlyProvided()
        {
            // Act
            var articles = await _helpArticleRepository.GetAllArticlesFilteredAsync(keyword: "Article");

            // Assert
            articles.Should().HaveCount(3); // Assuming all articles have "Article" in their title
        }

        [Fact]
        public async Task GetAllArticlesFilteredAsync_ReturnsAllArticles_WhenNoSortTypeOrKeywordProvided()
        {
            // Act
            var articles = await _helpArticleRepository.GetAllArticlesFilteredAsync();

            // Assert
            articles.Should().HaveCount(3); // Assuming no filtering is applied
        }

        [Fact]
        public async Task GetAllArticlesFilteredAsync_ReturnsEmptyList_WhenNoArticlesFound()
        {
            // Act
            var articles = await _helpArticleRepository.GetAllArticlesFilteredAsync(keyword: "Nonexistent");

            // Assert
            articles.Should().BeEmpty();
        }
    }
}
