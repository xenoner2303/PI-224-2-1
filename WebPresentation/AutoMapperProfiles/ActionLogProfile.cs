using AutoMapper;
using DTOsLibrary;
using BLL.EntityBLLModels;

namespace WebPresentation.AutoMapperProfiles;

public class ActionLogProfile : Profile
{
    public ActionLogProfile()
    {
        CreateMap<ActionLogModel, ActionLogDto>().ReverseMap();
    }
}
