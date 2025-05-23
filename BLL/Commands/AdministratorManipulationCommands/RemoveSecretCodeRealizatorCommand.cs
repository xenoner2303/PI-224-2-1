using AutoMapper;
using BLL.EntityBLLModels;
using DAL.Data;

namespace BLL.Commands;

internal class RemoveSecretCodeRealizatorCommand : AbstrCommandWithDA<bool>
{
    private readonly SecretCodeRealizatorModel realizatorModel;

    public override string Name => "Видалення автора";

    internal RemoveSecretCodeRealizatorCommand(SecretCodeRealizatorModel realizatorModel, IUnitOfWork operateUnitOfWork, IMapper mapper)
        : base(operateUnitOfWork, mapper)
    {
        ArgumentNullException.ThrowIfNull(realizatorModel, nameof(realizatorModel));

        this.realizatorModel = realizatorModel;
    }

    public override bool Execute()
    {
        dAPoint.SecretCodeRealizatorRepository.Remove(realizatorModel.Id);
        dAPoint.Save();

        LogAction($"Видалено реалізатор коду: {realizatorModel.Id}");
        return true;
    }
}