using FPT.DataAccess.Data;
using FPT.DataAccess.DbInitializer;
using FPT.DataAccess.Repository.IRepository;
using FPT.DataAccess.Repository;
using FPT.Models;
using FPTJobMatch.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace FPTJobMatch.Installers
{
    public class SystemInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            // Add MVC services
            services.AddControllersWithViews();

            // Add SignalR services
            services.AddSignalR();

            // Add Google authentication
            services.AddAuthentication().AddGoogle(googleOptions =>
            {
                googleOptions.ClientId = configuration["Authentication:Google:ClientId"];
                googleOptions.ClientSecret = configuration["Authentication:Google:ClientSecret"];
            });

            // Add Facebook authentication
            services.AddAuthentication().AddFacebook(facebookOptions =>
            {
                facebookOptions.AppId = configuration["Authentication:Facebook:AppId"];
                facebookOptions.AppSecret = configuration["Authentication:Facebook:AppSecret"];
            });

            // Add DbContext
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            // Add Identity
            services.AddIdentity<ApplicationUser, IdentityRole>()
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultTokenProviders();

            // Configure application cookie
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = $"/Access/Index";
                options.LogoutPath = $"/Access/Logout";
                options.AccessDeniedPath = $"/error/accessdenied";
            });
        }
    }
}