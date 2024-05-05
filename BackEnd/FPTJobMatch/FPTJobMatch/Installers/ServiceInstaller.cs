using FPT.DataAccess.DbInitializer;
using FPT.DataAccess.Repository;
using FPT.DataAccess.Repository.IRepository;
using FPTJobMatch.Services;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace FPTJobMatch.Installers
{
    public class ServiceInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            // Add scoped services
            services.AddScoped<IDbInitializer, DbInitializer>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Add singleton service
            services.AddSingleton<IEmailSender, EmailService>();
        }
    }
}