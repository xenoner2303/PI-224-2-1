﻿using AutoMapper;
using BLL.EntityBLLModels;
using DAL.Data;

namespace BLL.Commands;

internal class LoadSecretCodeRealizatorsCommand : AbstrCommandWithDA<List<SecretCodeRealizatorModel>>
{
    public override string Name => "Команда із завантаження реалізаторів кодів реєстрації";

    internal LoadSecretCodeRealizatorsCommand(IUnitOfWork operateUnitOfWork, IMapper mapper)
        : base(operateUnitOfWork, mapper) { }

    public override List<SecretCodeRealizatorModel> Execute()
    {
        var realizators = dAPoint.SecretCodeRealizatorRepository.GetAll();

        LogAction($"Було завантажено {realizators.Count} реалізаторів секретних кодів");

        return mapper.Map<List<SecretCodeRealizatorModel>>(realizators);
    }
}
