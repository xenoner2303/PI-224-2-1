using AutoMapper;
using BLL.EntityBLLModels;
using DAL.Data;

namespace BLL.Commands;

public class LoadLogsCommand : AbstrCommandWithDA<List<ActionLogModel>>
{
    public override string Name => "Команда із завантаження логів";
    private DateTime? timelog;

    internal LoadLogsCommand(DateTime? timelog, IUnitOfWork operateUnitOfWork, IMapper mapper)
        : base(operateUnitOfWork, mapper)
    {
        this.timelog = timelog;
    }

    public override List<ActionLogModel> Execute()
    {
        var logs = dAPoint.ActionLogRepository.GetAll();

        if (timelog == null)
        {
            LogAction($"Було завантажено всі {logs.Count} логів");
            return mapper.Map<List<ActionLogModel>>(logs);  //автомаперимо
        }

        var certainTimeLogs = logs
            .Where(x => x.ActionTime.Date == timelog.Value.Date)
            .ToList();

        LogAction($"Було завантажено {certainTimeLogs.Count} логів дати {timelog?.ToString("dd.MM.yyyy")}");
        return mapper.Map<List<ActionLogModel>>(certainTimeLogs);  //автомаперимо
    }
}
