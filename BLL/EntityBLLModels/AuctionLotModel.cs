using DAL.Entities;

namespace BLL.EntityBLLModels;

public class AuctionLotModel
{
    public int Id { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public decimal StartPrice { get; set; }

    public EnumLotStatuses Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public int OwnerId { get; set; }

    public int CategoryId { get; set; }
    public int? ManagerId { get; set; }

    public List<BidModel> Bids { get; set; } = new List<BidModel>();
}
