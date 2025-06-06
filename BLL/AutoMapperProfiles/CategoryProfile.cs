﻿using AutoMapper;
using BLL.EntityBLLModels;
using DAL.Entities;

namespace BLL.AutoMapperProfiles;

public class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        CreateMap<Category, CategoryModel>().ReverseMap();
    }
}
