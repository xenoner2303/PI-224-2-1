namespace BLL.EntityBLLModels;

public enum BusinessEnumLotStatuses
{
    Pending, // очікує на перевірку
    Active, // активний
    Completed, // завершений
    Rejected // відхилений
}
