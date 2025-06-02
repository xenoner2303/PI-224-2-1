using BLL.AutoMapperProfiles;
using BLL.AutoMapperProfiles.ValueResolvers;
using BLL.Commands;
using BLL.Commands.ManagerManipulationCommands;
using BLL.Commands.PreUsersManipulationCommands;
using BLL.Commands.UserManipulationsCommands;
using DAL.Data;
using Microsoft.Extensions.DependencyInjection;

namespace BLL.Services;

public static class BLLInitializer
{
    public static void AddAutoMapperToServices(IServiceCollection services)
    {
        // реєстрація автомаперу
        services.AddAutoMapper(typeof(UserProfile).Assembly);
    }

    public static void AddCommandDependenciesToServices(IServiceCollection services)
    {
        // ініціалізація залежностей рівня дал
        DALInitializer.AddDataAccessServices(services);
        
        services.AddScoped<PreUserCommandsManager>();
        services.AddScoped<ManagerCommandsManager>();
        services.AddScoped<AdministratorCommandsManager>();
        services.AddScoped<UserCommandManager>();
        services.AddScoped<BytesToImageResolver>();
        services.AddScoped<ImageToBytesImageModelResolver>();
    }
}
