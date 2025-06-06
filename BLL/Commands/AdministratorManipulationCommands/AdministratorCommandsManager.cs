﻿using AutoMapper;
using BLL.EntityBLLModels;
using DAL.Data;
using DAL.Entities;

namespace BLL.Commands;

public class AdministratorCommandsManager : AbstractCommandManager
{
    public AdministratorCommandsManager(IUnitOfWork unitOfWork, IMapper mapper)
        : base(unitOfWork, mapper) { }

    public List<ActionLogModel> LoadLogs(DateTime? logTime)
    {
        var command = new LoadLogsCommand(logTime, unitOfWork, mapper);
        return ExecuteCommand(command, "Не вдалося завантажити логи");
    }

    public List<SecretCodeRealizatorModel> LoadSecretCodeRealizators()
    {
        var command = new LoadSecretCodeRealizatorsCommand(unitOfWork, mapper);
        return ExecuteCommand(command, "Не вдалося завантажити реалізаторів кодів доступу");
    }

    public List<BaseUserModel> LoadUsers()
    {
        var command = new LoadUsersCommand(unitOfWork, mapper);
        return ExecuteCommand(command, "Не вдалося завантажити користувачів");
    }

    public bool RemoveSecretCodeRealizator(SecretCodeRealizatorModel realizatorModel)
    {
        var command = new RemoveSecretCodeRealizatorCommand(realizatorModel, unitOfWork, mapper);
        return ExecuteCommand(command, "Не вдалося видалити реалізатора");
    }

    public bool CreateCodeRealizator(SecretCodeRealizatorModel realizatorModel)
    {
        var command = new CreateCodeRealizatorCommand(realizatorModel, unitOfWork, mapper);
        return ExecuteCommand(command, "Не вдалося створити реалізатора");
    }

    public bool SaveUsersChanges(List<BaseUserModel> users)
    {
        var command = new SaveUsersChangesCommand(users, unitOfWork, mapper);
        return ExecuteCommand(command, "Не вдалося зберегти зміни користувачів");
    }

    public bool RemoveUser(BaseUserModel userModel)
    {
        var command = new RemoveUserCommand(userModel, unitOfWork, mapper);
        return ExecuteCommand(command, "Не вдалося видалити користувача");
    }

    public BaseUserModel? GetUserById(int id)
    {
        try
        {
            var user = unitOfWork.UserRepository.GetById(id);
            return user != null ? mapper.Map<BaseUserModel>(user) : null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка при отриманні користувача: {ex.Message}");
            return null;
        }
    }

    public SecretCodeRealizatorModel? GetSecretCodeRealizatorById(int id)
    {
        try
        {
            var realizator = unitOfWork.SecretCodeRealizatorRepository.GetById(id);
            return realizator != null ? mapper.Map<SecretCodeRealizatorModel>(realizator) : null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка при отриманні реалізатора: {ex.Message}");
            return null;
        }
    }
}
