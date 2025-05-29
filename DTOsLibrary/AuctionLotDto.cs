using DTOsLibrary.DTOEnums;

namespace DTOsLibrary;

public class AuctionLotDto
{
    public int Id { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public decimal StartPrice { get; set; }

    public byte[]? ImageBytes { get; set; } // зберігаємо зображення як Base64 рядок для передачі через API

    public EnumLotStatusesDto Status { get; set; }

    public DateTime? StartTime { get; set; }
    public int DurationDays { get; set; }
    public DateTime? EndTime { get; set; }

    public BaseUserDto Owner { get; set; }
    public CategoryDto Category { get; set; }
    public BaseUserDto Manager { get; set; }

    public List<BidDto> Bids { get; set; } = new List<BidDto>();
}
