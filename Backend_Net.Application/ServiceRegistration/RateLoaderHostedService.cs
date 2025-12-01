using Backend_Net.Application.Common.Interfaces.Repositories;
using Backend_Net.Application.Services.Rate;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Backend_Net.Application.ServiceRegistration;

public class RateLoaderHostedService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IRateService _rateService;

    public RateLoaderHostedService(IServiceProvider serviceProvider, IRateService rateService)
    {
        _serviceProvider = serviceProvider;
        _rateService = rateService;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var rates = await uow.CurrencyRate
            .Where(x => true)
            .ToListAsync(cancellationToken);

        _rateService.SetRates(rates);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}