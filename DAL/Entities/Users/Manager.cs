namespace DAL.Entities;

public class Manager : AbstractUser
{
    public List<AuctionLot> ManagedLots { get; set; } = new List<AuctionLot>(); // 1 менеджер - багато лотів, які веде

    public override EnumUserInterfaceType InterfaceType => EnumUserInterfaceType.Manager;

    public Manager() : base() { }
}
