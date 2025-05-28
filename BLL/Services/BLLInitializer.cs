using BLL.AutoMapperProfiles;
using BLL.Commands.UsersManipulationCommands;
using DAL.Data;
using DAL.Data.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BLL.Services;

public static class BLLInitializer
{
    public static void AddAutoMapperToServices(IServiceCollection services)
    {
        // реєстрація автомаперу
        services.AddAutoMapper(typeof(UserProfile),
            typeof(ActionLogProfile),
            typeof(SecretCodeRealizatorProfile),
            typeof(BidProfile),
            typeof(CategoryProfile),
            typeof(AuctionLotProfile));
    }

    public static void AddCommandDependenciesToServices(IServiceCollection services)
    {
        // реєстрація контексту для автостворення юніту
        services.AddDbContext<AuctionDbContext>();

        // реєстрація UnitOfWork
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // реєстрація сервісу для збереження-клонування зображень
        services.AddScoped<IImageService>(provider => new ImageService("Images")); // директорією буде Images

        services.AddScoped<UserCommandsManager>();
    }
}
