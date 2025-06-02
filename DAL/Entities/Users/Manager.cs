namespace DAL.Entities;

public class Manager : AbstractUser
{
    public override EnumUserInterfaceType InterfaceType => EnumUserInterfaceType.Manager;

    public Manager() : base() { }
}
