namespace DAL.Entities;

public enum EnumLotStatuses
{
    Pending, // очікує на перевірку
    Active, // активний
    Completed, // завершений
    Rejected // відхилений
}
