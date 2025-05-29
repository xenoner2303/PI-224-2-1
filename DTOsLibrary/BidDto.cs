namespace DTOsLibrary;

public class BidDto
{
    public int Id { get; set; }

    public decimal Amount { get; set; }

    public DateTime PlacedAt { get; set; }

    public AuctionLotDto Lot { get; set; }

    public BaseUserDto User { get; set; }

    public override string ToString() => $"{User.Login} - {Amount:C} ({PlacedAt:dd.MM.yyyy HH:mm})";
}
