namespace br.com.fiap.cloudgames.Payment.Application.Consumers;

public interface IMessageConsumer
{
    Task ConsumeAsync();
}