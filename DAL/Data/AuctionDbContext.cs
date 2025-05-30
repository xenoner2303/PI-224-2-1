using Microsoft.EntityFrameworkCore;
using DAL.Entities;

namespace DAL.Data;

public class AuctionDbContext : DbContext
{
    public DbSet<AuctionLot> AuctionLots { get; set; }
    public DbSet<Bid> Bids { get; set; }
    public DbSet<Category> Categories { get; set; }

    public DbSet<AbstractUser> Users { get; set; }
    public DbSet<RegisteredUser> RegisteredUsers { get; set; }
    public DbSet<Manager> Managers { get; set; }
    public DbSet<Administrator> Administrators { get; set; }

    public DbSet<ActionLog> ActionLogs { get; set; }
    public DbSet<AbstractSecretCodeRealizator> SecretCodeRealizators { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=C:\\Users\\Zver\\source\\repos\\xenoner2303\\InternetAuction\\DAL\\Auction.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // TPH для юзерів
        modelBuilder.Entity<AbstractUser>()
            .HasDiscriminator<EnumUserInterfaceType>("UserType")
            .HasValue<Administrator>(EnumUserInterfaceType.Administrator)
            .HasValue<Manager>(EnumUserInterfaceType.Manager)
            .HasValue<RegisteredUser>(EnumUserInterfaceType.Registered);

        modelBuilder.Entity<AbstractUser>().Ignore(u => u.InterfaceType);

        modelBuilder.Entity<Bid>()
            .HasOne(b => b.Lot)
            .WithMany(l => l.Bids)
            .HasForeignKey(b => b.LotId)
            .OnDelete(DeleteBehavior.Cascade); // при видаленні лоту всі ставки пов’язані з ним будуть вилдалені

        // 1 користувач має багато ставок
        modelBuilder.Entity<Bid>()
            .HasOne(b => b.User)
            .WithMany(u => u.Bids)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Cascade); // при видаленні користувача всі його ставки також будуть вилдалені

        // 1 користувач має багато власних лотів
        modelBuilder.Entity<AuctionLot>()
            .HasOne(l => l.Owner)
            .WithMany(u => u.OwnLots)
            .HasForeignKey(l => l.OwnerId)
            .OnDelete(DeleteBehavior.Restrict); // не можна видаляти користувача, якщо у нього є лоти

        // 1 категорія має багато лотів
        modelBuilder.Entity<AuctionLot>()
            .HasOne(l => l.Category)
            .WithMany(c => c.Lots)
            .HasForeignKey(l => l.CategoryId)
            .OnDelete(DeleteBehavior.Restrict); // не можна видаляти категорію, якщо у неї є лоти

        // категорії-підкатегорії
        modelBuilder.Entity<Category>()
            .HasOne(c => c.Parent)
            .WithMany(c => c.Subcategories)
            .HasForeignKey(c => c.ParentId)
            .OnDelete(DeleteBehavior.Restrict); // не можна видаляти батьківську категорію, якщо у неї є підкатегорії

        // 1 менеджер має багато лотів
        modelBuilder.Entity<AuctionLot>()
            .HasOne<Manager>()
            .WithMany(m => m.ManagedLots)
            .HasForeignKey(x => x.ManagerId)
            .IsRequired(false); // менеджер не обов'язковий, бо є стан коли ще ніхто не підвердив лот

        // реалізатори через TPH
        modelBuilder.Entity<AbstractSecretCodeRealizator>()
           .HasDiscriminator<string>("RealizatorType")
           .HasValue<AdministratorSecretCodeRealization>("Administrator")
           .HasValue<ManagerSecretCodeRealizator>("Manager");
    }
}
