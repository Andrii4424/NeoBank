using Bank.API.Domain.RepositoryContracts;
using Bank.API.Domain.RepositoryContracts.Users;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

public class CreditInterestBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CreditInterestBackgroundService> _logger;

    public CreditInterestBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<CreditInterestBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Credit interest background service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessCredits();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating credit interest");
            }

            await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
        }
    }

    private async Task ProcessCredits()
    {
        using var scope = _serviceProvider.CreateScope();

        var creditRepository = scope.ServiceProvider.GetRequiredService<IUserCreditsRepository>();

        var activeCredits = await creditRepository.GetActiveCreditsAsync();

        foreach (var credit in activeCredits)
        {
            var dailyRate = credit.InterestRate / 100 / 365;

            var interest = credit.RemainingDebt * dailyRate;

            credit.RemainingDebt += interest;

            credit.CurrentMonthAmountDue += interest;
        }

        await creditRepository.SaveAsync();

        _logger.LogInformation("Credit interest calculated for {Count} credits", activeCredits.Count);
    }
}