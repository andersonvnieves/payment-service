using System;
using System.Collections.Generic;
using System.Text;

namespace br.com.fiap.cloudgames.Payment.Infrastructure.Config
{
    public class RabbitMqQueueDetailsSettings
    {
        public string Exchange { get; set; }
        public string RoutingKey { get; set; }
    }
}
