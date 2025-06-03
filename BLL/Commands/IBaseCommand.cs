namespace BLL.Commands;

internal interface IBaseCommand<TResult>
{
    public string Name { get; }
    public TResult Execute();
}
