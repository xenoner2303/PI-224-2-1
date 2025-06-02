using AutoMapper;
using DAL.Data;

namespace BLL.Commands;

public class RemoveSecretCodeRealizatorByIdCommand : AbstrCommandWithDA<bool>
{
    private int id;

    public override string Name => "Видалення автора";

    internal RemoveSecretCodeRealizatorByIdCommand(int id, IUnitOfWork operateUnitOfWork, IMapper mapper)
        : base(operateUnitOfWork, mapper)
    {
        if (id <= 0)
        {
            throw new ArgumentException("ID не може бути менше або дорівнювати нулю", nameof(id));
        }

        this.id = id;
    }

    public override bool Execute()
    {
        dAPoint.SecretCodeRealizatorRepository.Remove(id);
        dAPoint.Save();

        LogAction($"Видалено реалізатор коду з айді {id}");
        return true;
    }
}