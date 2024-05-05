using FPT.DataAccess.Data;
using FPT.DataAccess.Repository.IRepository;
using FPT.DataAccess.Repository;
using FPT.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;

namespace FPT.Tests.Repositories
{
    public class JobRepositoryTests : IDisposable
    {
        private ApplicationDbContext _context;
        private IJobRepository _jobRepository;

        public JobRepositoryTests()
        {
            _context = GetDbContext().Result;
            _jobRepository = new JobRepository(_context);
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

            // Seed some cities
            var city1 = new City { Id = 1, Name = "City 1" };
            var city2 = new City { Id = 2, Name = "City 2" };
            context.Cities.AddRange(new List<City> { city1, city2 });
            await context.SaveChangesAsync();

            // Seed some companies associated with cities
            var company1 = new Company { Id = 1, Name = "Company 1", CityId = city1.Id };
            var company2 = new Company { Id = 2, Name = "Company 2", CityId = city2.Id };
            context.Companies.AddRange(new List<Company> { company1, company2 });
            await context.SaveChangesAsync();

            // Set the deadline as one day ahead from the current date
            var deadline = DateOnly.FromDateTime(DateTime.Now).AddDays(1);

            // Seed some jobs for testing and associate them with companies
            context.Jobs.AddRange(new List<Job>
            {
                new Job { Id = 1, Title = "Job 1", Address = "Address 1", CreatedAt = DateTime.UtcNow, Deadline = deadline, JobTypeId = 1, EmployerId = "employer1", CompanyId = company1.Id },
                new Job { Id = 2, Title = "Job 2", Address = "Address 2", CreatedAt = DateTime.UtcNow, Deadline = deadline, JobTypeId = 2, EmployerId = "employer2", CompanyId = company2.Id },
                new Job { Id = 3, Title = "Job 3", Address = "Address 3", CreatedAt = DateTime.UtcNow, Deadline = deadline, JobTypeId = 1, EmployerId = "employer1", CompanyId = company1.Id }
            });

            await context.SaveChangesAsync();

            return context;
        }

        [Fact]
        public async Task GetAllFilteredAsync_ReturnsCorrectJobs_WhenFilterAndIncludePropertiesProvided()
        {
            // Arrange
            Expression<Func<Job, bool>> filter = j => j.JobTypeId == 1;
            string includeProperties = "Company";

            // Act
            var jobs = await _jobRepository.GetAllFilteredAsync(filter, includeProperties, cityId: null, jobtypeId: null, keyword: null, pageIndex: 1);

            // Assert
            jobs.Should().HaveCount(2); // Assuming there are 2 jobs with JobTypeId = 1
            jobs.Should().OnlyContain(j => j.JobTypeId == 1);
            jobs.Should().NotContainNulls(j => j.Company);
        }

        [Fact]
        public async Task GetAllFilteredAsync_ReturnsCorrectJobs_WhenJobtypeIdProvided()
        {
            // Arrange
            int cityId = 2;
            int jobtypeId = 2;

            // Act
            var jobs = await _jobRepository.GetAllFilteredAsync(filter: null, includeProperties: null, cityId: cityId, jobtypeId, keyword: null, pageIndex: 1);

            // Assert
            jobs.Should().HaveCount(1); // Assuming there is 1 job with CityId = 1 and JobTypeId = 2
            jobs.Should().OnlyContain(j => j.JobTypeId == jobtypeId);
        }

        [Fact]
        public async Task GetAllFilteredAsync_ReturnsCorrectJobs_WhenKeywordProvided()
        {
            // Arrange
            string keyword = "Job";

            // Act
            var jobs = await _jobRepository.GetAllFilteredAsync(filter: null, includeProperties: null, cityId: null, jobtypeId: null, keyword, pageIndex: 1);

            // Assert
            jobs.Should().HaveCount(3); // Assuming all jobs have "Job" in their title
            jobs.Should().OnlyContain(j => j.Title.ToLower().Contains(keyword.ToLower()));
        }

        [Fact]
        public async Task RemoveRangeByEmployerIdAsync_RemovesJobsByEmployerId()
        {
            // Arrange
            string employerId = "employer1";

            // Act
            await _jobRepository.RemoveRangeByEmployerIdAsync(employerId);
            await _context.SaveChangesAsync();

            // Assert
            var jobs = await _context.Jobs.Where(job => job.EmployerId == employerId).ToListAsync();
            jobs.Should().BeEmpty();
        }

        [Fact]
        public async Task AddJob_WithCorrectData_ShouldAddJobToRepository()
        {
            // Arrange
            var newJob = new Job
            {
                Title = "Software Engineer",
                Address = "123 Main St",
                Description = "Lorem ipsum",
                CreatedAt = DateTime.Now,
                Deadline = DateOnly.FromDateTime(DateTime.Now.AddDays(7)),
                CategoryId = 1, // Assuming category with Id 1 exists in the database
                EmployerId = "employer1" // Assuming employer with Id "employer1" exists in the database
            };

            // Act
            _jobRepository.Add(newJob);
            await _context.SaveChangesAsync();

            // Assert
            var addedJob = _context.Jobs.FirstOrDefault(j => j.Title == "Software Engineer");
            addedJob.Should().NotBeNull();
            addedJob.Title.Should().Be("Software Engineer");
            // Add more assertions as needed
        }

        //[Fact]
        //public async Task AddJob_WithIncompleteData_ShouldNotAddJobToRepository()
        //{
        //    // Arrange: Create a job with incomplete data (missing required fields)
        //    var incompleteJob = new Job
        //    {
        //        // Missing Deadline, CategoryId, and EmployerId
        //        Title = "Incomplete Job",
        //        Address = "Some Address",
        //        Description = "Lorem ipsum",
        //        CreatedAt = DateTime.Now
        //    };

        //    // Act: Try to add the incomplete job
        //    _jobRepository.Add(incompleteJob);
        //    await _context.SaveChangesAsync();

        //    // Assert: Ensure that the job was not added to the repository
        //    var addedJob = _context.Jobs.FirstOrDefault(j => j.Title == "Incomplete Job");
        //    addedJob.Should().BeNull(); // The job should not exist in the database
        //}
    }
}
