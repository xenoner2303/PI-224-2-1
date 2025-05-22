using System.Text.RegularExpressions;

namespace DAL.Entities;

public abstract class AbstrUser
{
    private string login;
    private string firstName;
    private string lastName;
    private string email;
    private string phoneNumber;
    private int age;
    private string passwordHash;

    public int Id { get; set; } // айді для бази даних

    public string Login
    {
        get => this.login;
        set => this.login = string.IsNullOrWhiteSpace(value) ? throw new ArgumentException("Логін не може бути порожнім") : value;
    }

    public string FirstName
    {
        get => this.firstName;
        set => this.firstName = string.IsNullOrWhiteSpace(value) ? "Без імені" : value;
    }

    public string LastName
    {
        get => this.lastName;
        set => this.lastName = string.IsNullOrWhiteSpace(value) ? "Без прізвища" : value;
    }

    public string Email
    {
        get => this.email;
        set
        {
            if (!string.IsNullOrWhiteSpace(value) && !Regex.IsMatch(value, @"^\S+@\S+\.\S+$"))
            {
                throw new ArgumentException("Невірний формат пошти", value);
            }

            this.email = string.IsNullOrWhiteSpace(value) ? "Пошта відсутня" : value;
        }
    }

    public string PhoneNumber
    {
        get => this.phoneNumber;
        set
        {
            if (!string.IsNullOrWhiteSpace(value) && !Regex.IsMatch(value, @"^\+?\d{10,15}$"))
            {
                throw new ArgumentException("Невірний формат номеру телефону", value);
            }

            this.phoneNumber = string.IsNullOrWhiteSpace(value) ? "Номер відсутній" : value;
        }
    }

    public int Age
    {
        get => age;
        set => age = value < 0 ? throw new ArgumentOutOfRangeException(value.ToString(), "Вік не може бути меншим за 0") : value;
    }

    public string PasswordHash
    {
        get => passwordHash;
        set => passwordHash = string.IsNullOrWhiteSpace(value) ? throw new ArgumentNullException(nameof(PasswordHash)) : value;
    }

    public abstract EnumUserInterfaceType InterfaceType { get; }

    protected AbstrUser() { }

    public void CopyBaseFieldsFrom(AbstrUser other, bool copyId = false)
    {
        if (other == null)
        {
            throw new ArgumentNullException(nameof(other));
        }

        if (copyId)
        {
            this.Id = other.Id;
        }

        this.login = other.Login;
        this.firstName = other.FirstName;
        this.lastName = other.LastName;
        this.email = other.Email;
        this.phoneNumber = other.PhoneNumber;
        this.age = other.Age;
        this.passwordHash = other.PasswordHash;
    }
}
