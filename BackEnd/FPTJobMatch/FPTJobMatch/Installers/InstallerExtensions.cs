namespace FPTJobMatch.Installers
{
    public static class InstallerExtensions
    {
        // Extension method to install services defined in classes implementing IInstaller
        public static void InstallServicesInAssembly(this IServiceCollection services, IConfiguration configuration)
        {
            // Get all types exported by the assembly containing the Program class
            var installerTypes = typeof(Program).Assembly.ExportedTypes;

            // Select types that implement the IInstaller interface, are not interfaces themselves, and are not abstract
            var installers = installerTypes
                .Where(type => typeof(IInstaller).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                .Select(Activator.CreateInstance) // Create instances of the types using reflection
                .Cast<IInstaller>() // Cast the instances to IInstaller
                .ToList(); // Convert to a list

            // Call InstallServices method on each IInstaller instance
            installers.ForEach(installer => installer.InstallServices(services, configuration));
        }
    }
}
