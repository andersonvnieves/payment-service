using br.com.fiap.cloudgames.Payment.Infrastructure.Messagging.Consumers;

namespace br.com.fiap.cloudgames.Payment.WebAPI;

public class Worker: BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly OrderCreatedEventConsumer _orderCreatedEventConsumer;

    public Worker(ILogger<Worker> logger,
        OrderCreatedEventConsumer orderCreatedEventConsumer)
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