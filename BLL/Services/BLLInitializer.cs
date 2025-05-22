using BLL.AutoMapperProfiles;
using BLL.Commands.UsersManipulationCommands;
using DAL.Data;
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

        services.AddScoped<UserCommandsManager>();
    }
}
