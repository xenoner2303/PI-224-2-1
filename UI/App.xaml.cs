using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using UI.ApiClients;

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
        var startWindow = ServiceProvider.GetRequiredService<UserManagerWindow>();
        startWindow.Show();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        services.AddHttpClient<PreUserApiClient>(client =>
        {
            client.BaseAddress = new Uri("http://localhost:5000/api/PreUser/");
        });

        services.AddHttpClient<UserApiClient>(client =>
        {
            client.BaseAddress = new Uri("http://localhost:5000/api/User/");
        });

        services.AddHttpClient<ManagerApiClient>(client =>
        {
            client.BaseAddress = new Uri("http://localhost:5000/api/Manager/");
        });

        // реєструємо вікна
        services.AddTransient<UserManagerWindow>();

    }
}
