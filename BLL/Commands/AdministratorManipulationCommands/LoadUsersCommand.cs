using AutoMapper;
using BLL.EntityBLLModels;
using DAL.Data;

namespace BLL.Commands;

internal class LoadUsersCommand : AbstrCommandWithDA<List<BaseUserModel>>
{
    public override string Name => "Завантаження списку користувачів";

    internal LoadUsersCommand(IUnitOfWork operateUnitOfWork, IMapper mapper)
        : base(operateUnitOfWork, mapper) { }

    public override List<BaseUserModel> Execute()
    {
        var users = dAPoint.UserRepository.GetAll();

        LogAction($"Було завантажено {users.Count} користувачів");

        return mapper.Map<List<BaseUserModel>>(users);
    }
}
