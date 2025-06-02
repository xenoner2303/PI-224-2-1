using AutoMapper;
using DAL.Data;

namespace BLL.Commands;

public class RemoveUserByIdCommand : AbstrCommandWithDA<bool>
{
    private int id;

    public override string Name => "Видалення користувача";

    internal RemoveUserByIdCommand(int id, IUnitOfWork operateUnitOfWork, IMapper mapper)
        : base(operateUnitOfWork, mapper)
    {
        if(id <= 0)
        {
            throw new ArgumentException("ID не може бути менше або дорівнювати нулю", nameof(id));
        }

        this.id = id;
    }

    public override bool Execute()
    {
        dAPoint.UserRepository.Remove(id);
        dAPoint.Save();

        LogAction($"Користувача з айді {id} видалено");
        return true;
    }
}
