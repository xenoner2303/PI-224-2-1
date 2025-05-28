using DTOsLibrary.DTOEnums;

namespace DTOsLibrary;

public class SecretCodeRealizatorDto
{
    public int Id { get; set; }

    public string SecretCode { get; set; }

    public int CodeUses { get; set; }

    public EnumInterfaceTypeDto RealizatorType { get; set; }

    public override string ToString() => $"Тип: {RealizatorType.ToString()}, кількість використань: {CodeUses}";
}
