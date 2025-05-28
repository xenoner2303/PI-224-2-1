using AutoMapper;
using BLL.EntityBLLModels;
using DAL.Data;

namespace BLL.Commands.UserManipulationsCommands;

public class UserCommandManager : AbstractCommandManager
{
    public UserCommandManager(IUnitOfWork unitOfWork, IMapper mapper)
    : base(unitOfWork, mapper) { }

    
}
