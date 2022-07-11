using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace VueStart.Services;

public class StatisticsService : BackgroundService
{
    private readonly VisitorStatisticService visitorStatisticService;
    private readonly InputStatisticService inputStatisticService;
    private readonly IServiceProvider serviceProvider;
    private readonly Channel<EventData> eventChannel;
    private readonly ILogger<StatisticsService> logger;
    private int currentDay = -1;
    private int currentPeriod = -1;
    public StatisticsService(VisitorStatisticService visitorStatisticService, InputStatisticService inputStatisticService, IServiceProvider serviceProvider, Channel<EventData> eventChannel, ILogger<StatisticsService> logger, IHostApplicationLifetime applicationLifetime)
    {
        this.visitorStatisticService = visitorStatisticService;
        this.inputStatisticService = inputStatisticService;
        this.serviceProvider = serviceProvider;
        this.eventChannel = eventChannel;
        this.logger = logger;
        applicationLifetime.ApplicationStopping.Register(() => {
            logger.Log(LogLevel.Information, "Application stopping detected.");
            SaveData().Wait();
        });
    }

    private async Task OnEvent(EventData eventData)
    {
        try {
            var now = DateTime.UtcNow;
            #if DEBUG
            var periodLengthInMinutes = 1;
            #else
            var periodLengthInMinutes = 15;
            #endif
            int day = (now - new DateTime(2021, 1, 1)).Days;
            int period = (int)now.TimeOfDay.TotalMinutes / periodLengthInMinutes;
            if (currentDay != -1 && (day != currentDay || period != currentPeriod))
            {
                await SaveData();
            }
            currentDay = day;
            currentPeriod = period;
            visitorStatisticService.StoreVisit(eventData);
            inputStatisticService.StoreStatisticRecord(eventData.Request, eventData.ActionType, eventData.Error);
        } catch (Exception e) {
            using IServiceScope scope = serviceProvider.CreateScope();
            var errorHandlingService = scope.ServiceProvider.GetRequiredService<ErrorHandlerService>();
            errorHandlingService.OnException(e, null);
            logger.Log(LogLevel.Error, e, e.Message);
        }
    }

    private async Task SaveData()
    {
        using IServiceScope scope = serviceProvider.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        try {
            await visitorStatisticService.SaveVisitors(dbContext);
            inputStatisticService.SaveRecords(dbContext);
            dbContext.SaveChanges();
        } catch (Exception e) {
            var errorHandlingService = scope.ServiceProvider.GetRequiredService<ErrorHandlerService>();
            errorHandlingService.OnException(e, null);
            logger.Log(LogLevel.Error, e, e.Message);
        }
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach(var e in eventChannel.Reader.ReadAllAsync(stoppingToken)) {
            logger.Log(LogLevel.Information, "Event data read.");
            await OnEvent(e);
        }
        logger.Log(LogLevel.Information, "Event log finished.");
        await SaveData();
    }
}
