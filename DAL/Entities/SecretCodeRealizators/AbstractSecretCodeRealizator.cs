namespace DAL.Entities;

public abstract class AbstractSecretCodeRealizator
{
    public int Id { get; set; }

    private string secretCodeHash;
    public string SecretCodeHash
    {
        get => this.secretCodeHash;
        set
        {
            ArgumentNullException.ThrowIfNullOrEmpty(value, "Хеш-код секретного коду не може бути порожнім");
            secretCodeHash = value;
        }
    }

    protected int codeUses = 0;
    public int CodeUses
    {
        get => this.codeUses;
        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(CodeUses), "Кількість використань коду не може бути від'ємною");
            }

            this.codeUses = value;
        }
    }

    protected AbstractSecretCodeRealizator() { }

    protected AbstractSecretCodeRealizator(string secretCodeHash, int uses)
    {
        this.SecretCodeHash = secretCodeHash;
        this.CodeUses = uses;
    }

    public abstract EnumUserInterfaceType InterfaceType { get; }

    public abstract IUserBuilder ReturnTypeBuilder();
}