using DAL.Data.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DAL.Data;

public static class DALInitializer
{
    public static void AddDataAccessServices(IServiceCollection services)
    {
        // реєстрація контексту для автостворення юніту
        services.AddDbContext<AuctionDbContext>();

        // реєстрація UnitOfWork як реалізацію IUnitOfWork
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // реєстрація сервісу для збереження-клонування зображень
        services.AddScoped<IImageService>(provider => new ImageService("Images", "C:\\Users\\Zver\\source\\repos\\xenoner2303\\InternetAuction\\DAL\\Data\\")); // директорією буде Images
    }
}
