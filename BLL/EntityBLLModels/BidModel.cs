namespace BLL.EntityBLLModels;

public class BidModel
{
    public int Id { get; set; }

    public decimal Amount { get; set; }

    public DateTime PlacedAt { get; set; }

    public AuctionLotModel Lot { get; set; }

    public BaseUserModel User { get; set; }
}
