using System.Text.RegularExpressions;

namespace DAL.Entities;

public class UserBuilder<T> : IUserBuilder
    where T : AbstrUser, new()
{
    private T localUser;

    public UserBuilder()
    {
        localUser = new T();
    }

    public void SetUserFirstName(string userFirstName)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(userFirstName, "Ім'я користувача не може бути порожнім");
        localUser.FirstName = userFirstName;
    }

    public void SetUserLastName(string userLastName)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(userLastName, "Прізвище користувача не може бути порожнім");
        localUser.LastName = userLastName;
    }

    public void SetUserAge(int userAge)
    {
        if (userAge < 0)
        {
            throw new ArgumentException("Вік не може бути меншим за 0", nameof(userAge));
        }

        localUser.Age = userAge;
    }

    public void SetUserLogin(string userLogin)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(userLogin, "Логін користувача не може бути порожнім");
        localUser.Login = userLogin;
    }

    public void SetUserEmail(string userEmail)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(userEmail);

        if (!Regex.IsMatch(userEmail, @"^\S+@\S+\.\S+$"))
        {
            throw new ArgumentException("Невірний формат пошти", userEmail);
        }

        localUser.Email = userEmail;
    }

    public void SetPhoneNumber(string phoneNumber)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(phoneNumber);

        if (!Regex.IsMatch(phoneNumber, @"^\+?\d{10,15}$"))
        {
            throw new ArgumentException("Невірний формат номера телефону.", nameof(phoneNumber));
        }

        localUser.PhoneNumber = phoneNumber;
    }

    public void SetPassword(string passwordHash)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(passwordHash);

        if (passwordHash.Length < 8)
        {
            throw new ArgumentException("Пароль має містити принаймні 8 символів!");
        }

        localUser.PasswordHash = passwordHash;
    }

    public AbstrUser GetBuild()
    {
        if (string.IsNullOrEmpty(localUser.Login))
        {
            throw new InvalidOperationException("Логін є обов'язковим полем");
        }

        if (string.IsNullOrEmpty(localUser.FirstName))
        {
            throw new InvalidOperationException("Ім'я є обов'язковим полем");
        }

        if (string.IsNullOrEmpty(localUser.LastName))
        {
            throw new InvalidOperationException("Прізвище є обов'язковим полем");
        }

        AbstrUser builtUser = localUser;
        localUser = new(); // Reset to a new instance for further use

        return builtUser;
    }
}

