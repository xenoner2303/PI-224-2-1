using DAL.Data;
using DAL.Entities;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace AuctionLotStatusWorkerLibrary;

public class LotStatusCheckerWorker : BackgroundService
{
    private readonly ILogger<LotStatusCheckerWorker> _logger;
    private readonly IServiceProvider _serviceProvider;

    // логер автоматично підставиться, бо ASP.NET Core сам реєструє логер через Microsoft.Extensions.Logging
    public LotStatusCheckerWorker(ILogger<LotStatusCheckerWorker> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope(); // створюємо новий scope, щоб отримати новий екземпляр IUnitOfWork та інших сервісів
            var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var now = DateTime.UtcNow;
            var lotsToComplete = uow.AuctionLotRepository
                .GetQueryable()
                .Where(l => l.Status == EnumLotStatuses.Active && l.EndTime.HasValue && l.EndTime <= now)
                .ToList();

            foreach (var lot in lotsToComplete)
            {
                lot.Status = EnumLotStatuses.Completed;
                uow.AuctionLotRepository.Update(lot);

                _logger.LogInformation($"Лот з ID {lot.Id} завершено автоматично ({lot.Title})");// логуємо у внутрішній спец щоденник логів сервісу
            }

            if (lotsToComplete.Count > 0)
            {
                uow.Save();
            }

            await Task.Delay(TimeSpan.FromHours(1), stoppingToken); // затримка оновлення та перевірок година,
                                                                    // бо, враховуючи, що лоти можуть бути активними від 24 годин,
                                                                    // частіше перевіряти не має сенсу
        }
    }
}
