using AutoMapper;
using BLL.EntityBLLModels;
using DAL.Entities;

namespace BLL.AutoMapperProfiles;

public class ActionLogProfile : Profile
{
    public ActionLogProfile()
    {
        CreateMap<ActionLog, ActionLogModel>().ReverseMap();
    }
}

