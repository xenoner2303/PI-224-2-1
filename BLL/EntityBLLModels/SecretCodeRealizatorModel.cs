namespace BLL.EntityBLLModels;

public class SecretCodeRealizatorModel
{
    public int Id { get; set; }
    public string SecretCode { get; set; } // код, який буде хешуватися при збереженні
    public int CodeUses { get; set; }
    public BusinessEnumInterfaceType RealizatorType { get; set; }

    public override string ToString() => $"Тип доступу: {RealizatorType.ToString()}. Використання: {CodeUses}";
}
