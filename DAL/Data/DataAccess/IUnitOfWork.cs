using DAL.Entities;

namespace DAL.Data;

public interface IUnitOfWork : IDisposable
{
    public IGenericRepository<AbstractUser> UserRepository { get; }
    public IGenericRepository<Manager> ManagerRepository { get; }
    public IGenericRepository<RegisteredUser> RegisteredUserRepository { get; }

    public IGenericRepository<AuctionLot> AuctionLotRepository { get; }
    public IGenericRepository<Bid> BidRepository { get; }
    public IGenericRepository<Category> CategoryRepository { get; }

    public IGenericRepository<ActionLog> ActionLogRepository { get; }
    public IGenericRepository<AbstractSecretCodeRealizator> SecretCodeRealizatorRepository { get; }

    void Save();
}
