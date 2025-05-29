namespace BLL.EntityBLLModels;

public class AuctionLotModel
{
    public int Id { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public decimal StartPrice { get; set; }

    public BusinessEnumLotStatuses Status { get; set; }

    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    public BaseUserModel Owner { get; set; }
    public CategoryModel Category { get; set; }
    public BaseUserModel Manager { get; set; }

    public List<BidModel> Bids { get; set; } = new List<BidModel>();
}
