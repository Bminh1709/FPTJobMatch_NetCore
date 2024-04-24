using FPT.DataAccess.Data;
using FPT.DataAccess.Repository.IRepository;
using FPT.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;
        private UserManager<ApplicationUser> _userManager;
        public IApplicationUserRepository ApplicationUser { get; private set; }

        public IJobSeekerDetailRepository JobSeekerDetail { get; private set; }

        public ICompanyRepository Company { get; private set; }

        public ICategoryRepository Category { get; private set; }

        public IJobRepository Job { get; private set; }
        public IJobTypeRepository JobType { get; private set; }

        public ICityRepository City { get; private set; }

        public IApplicantCVRepository ApplicantCV { get; private set; }
        public INotificationRepository Notification { get; private set; }
        public IHelpTypeRepository HelpType { get; private set; }
        public IHelpArticleRepository HelpArticle { get; private set; }

        public UnitOfWork(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;

            ApplicationUser = new ApplicationUserRepository(_db, _userManager);
            JobSeekerDetail = new JobSeekerDetailRepository(_db);
            Company = new CompanyRepository(_db);
            Category = new CategoryRepository(_db);
            Job = new JobRepository(_db);
            JobType = new JobTypeRepository(_db);
            City = new CityRepository(_db);
            ApplicantCV = new ApplicantCVRepository(_db);
            Notification = new NotificationRepository(_db);
            HelpType = new HelpTypeRepository(_db);
            HelpArticle = new HelpArticleRepository(_db);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
