namespace DAL.Entities;

public class ActionLog
{
    private string actionName;
    private string description;

    public int Id { get; set; } // айді для бази даних

    public string ActionName
    {
        get => actionName;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Назва дії не може бути порожньою", nameof(ActionName));
            }

            this.actionName = value;
        }
    }

    public string Description
    {
        get => description;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Опис не може бути порожнім", nameof(Description));
            }

            this.description = value;
        }
    }

    public DateTime ActionTime { get; private set; }

    public ActionLog() { }

    public ActionLog(string actionName, string description)
    {
        this.ActionName = actionName;
        this.Description = description;
        this.ActionTime = DateTime.UtcNow;
    }
}
