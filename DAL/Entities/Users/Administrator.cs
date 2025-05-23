namespace DAL.Entities;

public class Administrator : AbstractUser
{
    public override EnumUserInterfaceType InterfaceType => EnumUserInterfaceType.Administrator;

    public Administrator() : base() { }
}
