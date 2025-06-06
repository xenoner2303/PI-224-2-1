﻿using BLL.AutoMapperProfiles;
using BLL.Commands.ManagerManipulationCommands;
using BLL.Commands.PreUsersManipulationCommands;
using BLL.Commands.UserManipulationsCommands;
using DAL.Data;
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

        services.AddScoped<PreUserCommandsManager>();
        services.AddScoped<UserCommandManager>();
        services.AddScoped<ManagerCommandsManager>();
    }
}
