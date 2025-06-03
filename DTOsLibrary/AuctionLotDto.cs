using DTOsLibrary.DTOEnums;

namespace DTOsLibrary;

public class AuctionLotDto
{
    public int Id { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public decimal StartPrice { get; set; }

    public ImageDto? Image { get; set; }

    public EnumLotStatusesDto Status { get; set; } 

    public DateTime? StartTime { get; set; }
    public int DurationDays { get; set; }
    public DateTime? EndTime { get; set; }
    public string? RejectionReason { get; set; } = string.Empty;
    public BaseUserDto Owner { get; set; }
    public CategoryDto Category { get; set; }

    public List<BidDto> Bids { get; set; } = new List<BidDto>();

    public decimal CurrentPrice => Bids.Count > 0 
        ? Bids.OrderByDescending(b => b.Amount).FirstOrDefault()?.Amount ?? StartPrice 
        : StartPrice;
    public string WinnerLogin => Status == EnumLotStatusesDto.Completed
        ? Bids.OrderByDescending(b => b.Amount).FirstOrDefault()?.User?.Login ?? "Немає"
        : "Лот ще не завершено";
}
