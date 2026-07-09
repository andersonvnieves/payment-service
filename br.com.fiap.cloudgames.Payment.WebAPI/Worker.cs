using br.com.fiap.cloudgames.Payment.Application.Consumers;

namespace br.com.fiap.cloudgames.Payment.WebAPI;

public class Worker: BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IOrderCreatedEventConsumer _orderCreatedEventConsumer;

    public Worker(ILogger<Worker> logger,
        IOrderCreatedEventConsumer orderCreatedEventConsumer)
    {
        _logger = logger;
        _orderCreatedEventConsumer = orderCreatedEventConsumer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("Starting Worker");

            await _orderCreatedEventConsumer.ConsumeAsync();
            
            await Task.Delay(Timeout.Infinite, stoppingToken);
            _logger.LogInformation("Stopping Worker");
        }
        catch (Exception ex) 
        {
            _logger.LogError(ex.Message);
        }
        finally
        {
            await _orderCreatedEventConsumer.DisposeAsync();
        }
    }
}