using BLL.EntityBLLModels;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace Presentation.UIHelpers;

//internal static class WindowFactory
//{
//    private static readonly Dictionary<BusinessEnumInterfaceType, Func<BaseUserModel, IServiceProvider, Window>> windowMap = new()
//    {
//        { BusinessEnumInterfaceType.Student, (user, serviceProvider) => new StudentManagerWindow(user, serviceProvider.GetRequiredService<StudentCommandsManager>()) },
//        { BusinessEnumInterfaceType.Teacher, (user, serviceProvider) => new TeacherManagerWindow(user, serviceProvider.GetRequiredService<TeacherCommandsManager>()) },
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

