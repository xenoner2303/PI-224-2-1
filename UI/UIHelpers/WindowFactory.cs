using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using DTOsLibrary;
using UI.ApiClients;
using DTOsLibrary.DTOEnums;
using OnlineAuction;

namespace Presentation.UIHelpers;

internal static class WindowFactory
{
    private static readonly Dictionary<EnumInterfaceTypeDto, Func<BaseUserDto, IServiceProvider, Window>> windowMap = new()
    {
        { EnumInterfaceTypeDto.Registered, (user, serviceProvider) => new UserManagerWindow(serviceProvider, serviceProvider.GetRequiredService<UserApiClient>()) }, // зареєстрований
        { EnumInterfaceTypeDto.Manager, (user, serviceProvider) => new ManagerWindow(serviceProvider.GetRequiredService<ManagerApiClient>()) },
        { EnumInterfaceTypeDto.Administrator, (user, serviceProvider) => new AdminWindow(serviceProvider) }
    };

    internal static Window CreateWindow(BaseUserDto user, IServiceProvider serviceProvider)
    {
        if(user == null)
        {
            throw new ArgumentNullException(nameof(user), "Користувач не може бути null");
        }

        if (windowMap.TryGetValue(user.InterfaceType, out var windowCreator))
        {
            return windowCreator(user, serviceProvider);
        }

        throw new ArgumentException("Невідомий тип інтерфейсу");
    }
}
