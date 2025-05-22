namespace DAL.Entities;

public class ManagerSecretCodeRealizator : AbstractSecretCodeRealizator
{
    private ManagerSecretCodeRealizator() : base() { }
    public ManagerSecretCodeRealizator(string secretCodeHash, int uses) : base(secretCodeHash, uses) { }

    public override IUserBuilder ReturnTypeBuilder()
    {
        base.codeUses--;

        return new UserBuilder<Manager>();
    }
}
