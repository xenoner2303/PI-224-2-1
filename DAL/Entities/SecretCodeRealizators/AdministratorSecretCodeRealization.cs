namespace DAL.Entities;

public class AdministratorSecretCodeRealization : AbstractSecretCodeRealizator
{
    public override EnumUserInterfaceType InterfaceType => EnumUserInterfaceType.Administrator;

    private AdministratorSecretCodeRealization() : base() { }
    public AdministratorSecretCodeRealization(string secretCodeHash, int uses) : base(secretCodeHash, uses) { }

    public override IUserBuilder ReturnTypeBuilder()
    {
        base.codeUses--;

        return new UserBuilder<Administrator>();
    }
}
