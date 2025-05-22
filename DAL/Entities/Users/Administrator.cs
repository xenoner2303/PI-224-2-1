namespace DAL.Entities;

public class Administrator : AbstrUser
{
    public override EnumUserInterfaceType InterfaceType => EnumUserInterfaceType.Administrator;

    public Administrator() : base() { }
}
