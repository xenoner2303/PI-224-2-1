using AutoMapper;
using BLL.EntityBLLModels;
using BLL.Services;
using DAL.Data;
using DAL.Entities;

namespace BLL.Commands;

public class CreateCodeRealizatorCommand : AbstrCommandWithDA<bool>
{
    private readonly SecretCodeRealizatorModel codeRealizatorModel;

    private readonly Dictionary<BusinessEnumInterfaceType, Func<string, int, AbstractSecretCodeRealizator>> factoryMap
    = new()
    {
        { BusinessEnumInterfaceType.Manager, (hash, uses) => new ManagerSecretCodeRealizator(hash, uses) },
        { BusinessEnumInterfaceType.Administrator, (hash, uses) => new AdministratorSecretCodeRealization(hash, uses) }
    };

    public override string Name => "Створення реалізатора секретного коду";

    internal CreateCodeRealizatorCommand(SecretCodeRealizatorModel codeRealizatorModel, IUnitOfWork operateUnitOfWork, IMapper mapper)
        : base(operateUnitOfWork, mapper)
    {
        ArgumentNullException.ThrowIfNull(codeRealizatorModel, nameof(codeRealizatorModel));

        this.codeRealizatorModel = codeRealizatorModel;
        ValidateModel();
    }

    public override bool Execute()
    {
        // створюємо реалізатор
        var realizator = CreateRealizatorInstance();

        // додаємо реалізатор до репозиторію
        dAPoint.SecretCodeRealizatorRepository.Add(realizator);
        dAPoint.Save();

        LogAction($"Реалізатор секретного коду створений: {realizator.GetType().Name}, Використання: {realizator.CodeUses}");

        return true;
    }

    private void ValidateModel()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(codeRealizatorModel.SecretCode))
        {
            errors.Add("Секретний код не може бути порожнім");
        }

        if (codeRealizatorModel.CodeUses < 1)
        {
            errors.Add("Кількість використань повинна бути більше 0");
        }

        if (errors.Count > 0)
        {
            throw new ArgumentException(string.Join(Environment.NewLine, errors));
        }
    }

    private AbstractSecretCodeRealizator CreateRealizatorInstance()
    {
        var hashedCode = PasswordHasher.HashPassword(codeRealizatorModel.SecretCode);

        if (!factoryMap.TryGetValue(codeRealizatorModel.RealizatorType, out var creator))
        {
            throw new ArgumentException("Невідомий тип реалізатора");
        }

        return creator(hashedCode, codeRealizatorModel.CodeUses);
    }
}