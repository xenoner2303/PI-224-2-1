using DTOsLibrary.DTOEnums;

namespace DTOsLibrary;

public class BaseUserDto
{
    public int Id { get; set; }
    public string Login { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public int Age { get; set; }
    public string? Password { get; set; } // відкрите поле для нехешованого паролю, яке потім буде хешуватися
    public string? SecretCode { get; set; }
    public EnumInterfaceTypeDto InterfaceType { get; set; }

    public override string ToString() => $"{FirstName} {LastName} ({Login})";
}
