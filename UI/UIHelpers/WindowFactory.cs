//using System.Windows;
//using BLL.EntityBLLModels;
//using BLL.Commands;
//using Microsoft.Extensions.DependencyInjection;

//namespace Presentation.UIHelpers;

//internal static class WindowFactory
//{
//    private static readonly Dictionary<BusinessEnumInterfaceType, Func<BaseUserModel, IServiceProvider, Window>> windowMap = new()
//    {
//    //    { null, (user, sp) => new UserWindow() },  // випадок незареєстрованого користувача користувача, тут зробити 2 перевантаження констору
//     //   { BusinessEnumInterfaceType.Registered, (user, sp) => new UserWindow(user) }, // зареєстрований
//   //     { BusinessEnumInterfaceType.Manager, (user, serviceProvider) => new ManagerManagerWindow(user, serviceProvider.GetRequiredService<TeacherCommandsManager>()) },
//        { BusinessEnumInterfaceType.Administrator, (user, serviceProvider) => new AdministratorManagerWindow(serviceProvider.GetRequiredService<AdministratorCommandsManager>()) }
//    };

//    internal static Window CreateWindow(BaseUserModel user, IServiceProvider serviceProvider)
//    {
//        if (windowMap.TryGetValue(user.InterfaceType, out var windowCreator))
//        {
//            return windowCreator(user, serviceProvider);
//        }

//        throw new ArgumentException("Невідомий тип інтерфейсу");
//    }
//}
