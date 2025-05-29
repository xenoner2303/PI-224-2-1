using BLL.Services;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace Presentation;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static IServiceProvider ServiceProvider { get; private set; }

    //метод викликається при старті застосунку
    private void Application_Startup(object sender, StartupEventArgs e)
    {
        //створюємо контейнер
        var serviceCollection = new ServiceCollection();

        //реєструємо всі сервіси
        ConfigureServices(serviceCollection);

        //створюємо провайдер сервісів
        ServiceProvider = serviceCollection.BuildServiceProvider();

        //запускаємо головне вікно
        var mainWindow = ServiceProvider.GetRequiredService<AuthorizationWindow>();
        mainWindow.Show();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        //додаємо AutoMapper до сервісів
        BLLInitializer.AddAutoMapperToServices(services);

        //додаємо залежності до сервісів
        BLLInitializer.AddCommandDependenciesToServices(services);

        // реєструємо вікна
        services.AddSingleton<AuthorizationWindow>();
    }
}
