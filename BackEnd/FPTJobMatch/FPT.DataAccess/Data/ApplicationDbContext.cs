﻿using FPT.Models;
using FPT.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace FPT.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Status> AccountStatuses { get; set; }
        public DbSet<JobSeekerDetail> JobSeekerDetails { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<JobType> JobTypes { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<ApplicantCV> ApplicantCVs { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Status>().HasData(
                new Status { Id = 1, Name = SD.StatusSuspending },
                new Status { Id = 2, Name = SD.StatusPending },
                new Status { Id = 3, Name = SD.StatusActive },
                new Status { Id = 4, Name = SD.StatusResponded }
            );

            builder.Entity<JobType>().HasData(
                new JobType { Id = 1, Name = "Part Time" },
                new JobType { Id = 2, Name = "Full Time" },
                new JobType { Id = 3, Name = "Remote" }
            );

            builder.Entity<City>().HasData(
                new City { Id = 1, Name = "Ho Chi Minh" },
                new City { Id = 2, Name = "Ha Noi" },
                new City { Id = 3, Name = "Da Nang" },
                new City { Id = 4, Name = "Others" }
            );
        }
    }
}
