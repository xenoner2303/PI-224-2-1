namespace DAL.Entities;

public interface IUserBuilder : IBuilder<AbstractUser>
{
    void SetUserFirstName(string userFirstName);
    void SetUserLastName(string userLastName);
    void SetUserLogin(string userLogin);
    void SetUserEmail(string userEmail);
    void SetUserAge(int userAge);
    void SetPhoneNumber(string phoneNumber);
    void SetPassword(string password);
}
