using DAL.Entities;

namespace BLL.EntityBLLModels;

public class AuctionLotModel
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal StartPrice { get; set; }

    public ImageModel? Image { get; set; }

    public BusinessEnumLotStatuses Status { get; set; }
    public DateTime? StartTime { get; set; }
    public int DurationDays { get; set; }
    public DateTime? EndTime { get; set; }

    public int CategoryId { get; set; }
    public int? ManagerId { get; set; }

    public List<BidModel> Bids { get; set; } = new();
}
