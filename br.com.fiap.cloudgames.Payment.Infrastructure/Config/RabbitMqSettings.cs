using System;
using System.Collections.Generic;
using System.Text;

namespace br.com.fiap.cloudgames.Payment.Infrastructure.Config
{
    public class RabbitMqSettings
    {
        public required string URI { get; set; }
        public RabbitMqQueueDetailsSettings OrderCreatedEvent { get; set; }
        public RabbitMqQueueDetailsSettings PaymentProcessedEvent { get; set; }
    }
}
