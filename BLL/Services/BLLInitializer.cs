using BLL.AutoMapperProfiles;
using BLL.Commands.PreUsersManipulationCommands;
using DAL.Data;
using DAL.Data.Services;
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

        // реєстрація сервісу для збереження-клонування зображень
        services.AddScoped<IImageService>(provider => new ImageService("Images")); // директорією буде Images

        services.AddScoped<PreUserCommandsManager>();
    }
}
