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
    }
}
