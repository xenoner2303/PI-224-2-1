using DAL.Entities;

namespace DAL.Data;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<AbstrUser> UserRepository { get; }
    IGenericRepository<Manager> ManagerRepository { get; }
    IGenericRepository<RegisteredUser> RegisteredUserRepository { get; }

    IGenericRepository<AuctionLot> AuctionLotRepository { get; }
    IGenericRepository<Bid> BidRepository { get; }
    IGenericRepository<Category> CategoryRepository { get; }

    IGenericRepository<ActionLog> ActionLogRepository { get; }

    void Save();
}
