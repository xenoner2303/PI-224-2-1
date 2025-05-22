namespace BLL.Commands;

public interface IBaseCommand<TResult>
{
    public string Name { get; }
    public TResult Execute();
}
