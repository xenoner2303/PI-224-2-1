namespace DAL.Entities;

public class RegisteredUser : AbstrUser
{
    public List<AuctionLot> OwnLots { get; set; } = new(); // 1 користувач - багато його лотів
    public List<Bid> Bids { get; set; } = new List<Bid>(); // 1 користувач - багато його ставок

    public override EnumUserInterfaceType InterfaceType => EnumUserInterfaceType.Registered;

    public RegisteredUser() : base() { }
}
