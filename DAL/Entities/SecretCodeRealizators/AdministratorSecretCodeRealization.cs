namespace DAL.Entities;

public class AdministratorSecretCodeRealization : AbstractSecretCodeRealizator
{
    private AdministratorSecretCodeRealization() : base() { }
    public AdministratorSecretCodeRealization(string secretCodeHash, int uses) : base(secretCodeHash, uses) { }

    public override IUserBuilder ReturnTypeBuilder()
    {
        base.codeUses--;

        return new UserBuilder<Administrator>();
    }
}
