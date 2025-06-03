using AutoMapper;
using BLL.EntityBLLModels;
using BLL.Services;
using DAL.Data;
using DAL.Entities;

namespace BLL.Commands;

internal class SaveUsersChangesCommand : AbstrCommandWithDA<bool>
{
    private readonly List<BaseUserModel> users;
    private readonly Dictionary<BusinessEnumInterfaceType, Func<AbstractUser>> userFactoryMap
    = new()
    {
        { BusinessEnumInterfaceType.Registered, () => new RegisteredUser() },
        { BusinessEnumInterfaceType.Manager, () => new Manager() },
        { BusinessEnumInterfaceType.Administrator, () => new Administrator() }
    };

    public override string Name => "Збереження змін користувачів";

    internal SaveUsersChangesCommand(List<BaseUserModel> users, IUnitOfWork operateUnitOfWork, IMapper mapper)
        : base(operateUnitOfWork, mapper)
    {
        ArgumentNullException.ThrowIfNull(users);

        this.users = users;
    }

    //оскільки це команда адміністратора, то валідацію не робимо
    public override bool Execute()
    {
        int updatedCount = 0;
        int roleChangedCount = 0;

        foreach (var userModel in users)
        {
            var dbUser = dAPoint.UserRepository.GetById(userModel.Id);

            if (dbUser == null)
            {
                continue;
            }
            bool isChanged = false;

            if (dbUser.Login != userModel.Login)
            {
                dbUser.Login = userModel.Login;
                isChanged = true;
            }

            if (dbUser.FirstName != userModel.FirstName)
            {
                dbUser.FirstName = userModel.FirstName;
                isChanged = true;
            }

            if (dbUser.LastName != userModel.LastName)
            {
                dbUser.LastName = userModel.LastName;
                isChanged = true;
            }

            if (dbUser.Email != userModel.Email)
            {
                dbUser.Email = userModel.Email;
                isChanged = true;
            }

            if (dbUser.PhoneNumber != userModel.PhoneNumber)
            {
                dbUser.PhoneNumber = userModel.PhoneNumber;
                isChanged = true;
            }

            if (dbUser.Age != userModel.Age)
            {
                dbUser.Age = userModel.Age;
                isChanged = true;
            }

            // якщо пароль був змінений і не порожній — хешуємо і оновлюємо
            if (!string.IsNullOrWhiteSpace(userModel.Password))
            {
                var newHash = PasswordHasher.HashPassword(userModel.Password);
                if (dbUser.PasswordHash != newHash)
                {
                    dbUser.PasswordHash = newHash;
                    isChanged = true;
                }
            }

            if (isChanged)
            {
                dAPoint.UserRepository.Update(dbUser);
                updatedCount++;
            }

            //після зміни базових параметрів, перевіряємо зміну рівня аккаунту, бо якщо б ми зробили це перед цим, то була б помилка очікування зміни
            bool roleChanged = !dbUser.InterfaceType.ToString().Equals(userModel.InterfaceType.ToString());

            if (roleChanged)
            {
                ChangeUserRole(userModel.Id, userModel.InterfaceType);
                roleChangedCount++;
            }
        }

        dAPoint.Save();
        LogAction($"Оновлено {updatedCount} користувачів, змінено роль у {roleChangedCount} користувачів");

        return true;
    }

    private void ChangeUserRole(int userId, BusinessEnumInterfaceType newRole)
    {
        var oldUser = dAPoint.UserRepository.GetById(userId);

        if (oldUser == null)
        {
            throw new InvalidOperationException("Користувача не знайдено");
        }

        if (!userFactoryMap.TryGetValue(newRole, out var creator))
        {
            throw new ArgumentException("Невідомий тип користувача");
        }

        var newUser = creator();
        newUser.CopyBaseFieldsFrom(oldUser);

        // видаляємо старого
        dAPoint.UserRepository.Remove(oldUser.Id);

        // додаємо його заміну нового типу
        dAPoint.UserRepository.Add(newUser);

        dAPoint.Save();
        LogAction($"Користувач {oldUser.Login} змінено на роль {newRole}");
    }
}
