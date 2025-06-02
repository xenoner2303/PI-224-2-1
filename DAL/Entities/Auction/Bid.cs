namespace DAL.Entities;

public class Bid
{
    private decimal amount;

    public int Id { get; set; } // айді для бази даних

    public decimal Amount
    {
        get => amount;
        set
        {
            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(Amount), "Сума ставки має бути > 0");
            }

            amount = value;
        }
    }

    public DateTime PlacedAt { get; set; }

    public int LotId { get; set; }
    public AuctionLot Lot { get; set; }

    public int UserId { get; set; }
    public RegisteredUser User { get; set; }
}
