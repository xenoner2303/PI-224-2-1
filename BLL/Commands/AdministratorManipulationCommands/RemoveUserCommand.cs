using AutoMapper;
using BLL.EntityBLLModels;
using DAL.Data;

namespace BLL.Commands;

internal class RemoveUserCommand : AbstrCommandWithDA<bool>
{
    private readonly BaseUserModel userModel;

    public override string Name => "Видалення користувача";

    internal RemoveUserCommand(BaseUserModel userModel, IUnitOfWork operateUnitOfWork, IMapper mapper)
        : base(operateUnitOfWork, mapper)
    {
        ArgumentNullException.ThrowIfNull(userModel, nameof(userModel));

        this.userModel = userModel;
    }

    public override bool Execute()
    {
        dAPoint.UserRepository.Remove(userModel.Id);
        dAPoint.Save();

        LogAction($"Користувача {userModel.Login} видалено");
        return true;
    }
}
