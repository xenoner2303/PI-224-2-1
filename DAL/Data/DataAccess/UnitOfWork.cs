using DAL.Entities;

namespace DAL.Data;

public class UnitOfWork : IUnitOfWork
{
    private bool disposedValue;

    private readonly AuctionDbContext context;
    private readonly IGenericRepository<AbstractUser> userRepository;
    private readonly IGenericRepository<Manager> managerRepository;
    private readonly IGenericRepository<RegisteredUser> registeredUserRepository;
    private readonly IGenericRepository<AuctionLot> auctionLotRepository;
    private readonly IGenericRepository<Bid> bidRepository;
    private readonly IGenericRepository<Category> categoryRepository;
    private readonly IGenericRepository<ActionLog> actionLogRepository;
    private readonly IGenericRepository<AbstractSecretCodeRealizator> secretCodeRealizatorRepository;

    public IGenericRepository<AbstractUser> UserRepository => userRepository ?? new GenericRepository<AbstractUser>(context);
    public IGenericRepository<Manager> ManagerRepository => managerRepository ?? new GenericRepository<Manager>(context);
    public IGenericRepository<RegisteredUser> RegisteredUserRepository => registeredUserRepository ?? new GenericRepository<RegisteredUser>(context);

    public IGenericRepository<AuctionLot> AuctionLotRepository => auctionLotRepository ?? new GenericRepository<AuctionLot>(context);
    public IGenericRepository<Bid> BidRepository => bidRepository ?? new GenericRepository<Bid>(context);
    public IGenericRepository<Category> CategoryRepository => categoryRepository ?? new GenericRepository<Category>(context);

    public IGenericRepository<ActionLog> ActionLogRepository => actionLogRepository ?? new GenericRepository<ActionLog>(context);
    public IGenericRepository<AbstractSecretCodeRealizator> SecretCodeRealizatorRepository => secretCodeRealizatorRepository ?? new GenericRepository<AbstractSecretCodeRealizator>(context);


    public UnitOfWork(AuctionDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context, nameof(context));

        this.context = context;
    }

    public void Save()
    {
        context.SaveChanges();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                context.Dispose();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

