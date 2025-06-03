using AutoMapper;
using DAL.Data;
using DAL.Entities;

namespace BLL.Commands;

internal abstract class AbstrCommandWithDA<TResult> : IBaseCommand<TResult>
{
    protected readonly IUnitOfWork dAPoint;
    protected readonly IMapper mapper;

    public abstract string Name { get; }

    protected AbstrCommandWithDA(IUnitOfWork operateUnitOfWork, IMapper mapper)
    {
        this.dAPoint = operateUnitOfWork;
        this.mapper = mapper;
    }

    protected void LogAction(string description)
    {
        var logEntry = new ActionLog(this.Name, description);

        dAPoint.ActionLogRepository.Add(logEntry);
        dAPoint.Save();
    }

    public abstract TResult Execute();
}
