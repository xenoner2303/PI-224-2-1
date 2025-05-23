namespace DAL.Entities;

public class ManagerSecretCodeRealizator : AbstractSecretCodeRealizator
{
    public override EnumUserInterfaceType InterfaceType => EnumUserInterfaceType.Manager;

    private ManagerSecretCodeRealizator() : base() { }
    public ManagerSecretCodeRealizator(string secretCodeHash, int uses) : base(secretCodeHash, uses) { }

    public override IUserBuilder ReturnTypeBuilder()
    {
        base.codeUses--;

        return new UserBuilder<Manager>();
    }
}
