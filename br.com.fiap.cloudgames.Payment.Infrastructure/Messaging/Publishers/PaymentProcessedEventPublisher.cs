using Amazon.SQS;
using Amazon.SQS.Model;
using br.com.fiap.cloudgames.Payment.Application.Events;
using br.com.fiap.cloudgames.Payment.Application.Publishers;
using br.com.fiap.cloudgames.Payment.Infrastructure.Config;
using br.com.fiap.cloudgames.Payment.Infrastructure.Messagging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace br.com.fiap.cloudgames.Payment.Infrastructure.Messaging.Publishers;

public class PaymentProcessedEventPublisher: RabbitMqMessagePublisher, IPaymentProcessedEventPublisher
{
    private readonly IAmazonSQS _sqsClient;
    private readonly IOptions<AwsSqsSettings> _sqsOptions;
    public PaymentProcessedEventPublisher(IAmazonSQS sqsClient, 
        IOptions<AwsSqsSettings> sqsOptions,
        IOptions<RabbitMqSettings> rabbitMqOptions,
        RabbitMqConnection rabbitMqConnection)
        : base(rabbitMqConnection, 
            rabbitMqOptions.Value.PaymentProcessedEvent.Exchange,
            rabbitMqOptions.Value.PaymentProcessedEvent.RoutingKey)
    {
        _sqsClient = sqsClient;
        _sqsOptions = sqsOptions;
    }

    public async Task PublishAsync(PaymentProcessedEvent message)
    {
        await base.PublishAsync<PaymentProcessedEvent>(message);

        var messageBody = JsonSerializer.Serialize(message);
        var request = new SendMessageRequest
        {
            QueueUrl = _sqsOptions.Value.PaymentProcessedQueueUrl,
            MessageBody = messageBody
        };
        await _sqsClient.SendMessageAsync(request);
    }
}