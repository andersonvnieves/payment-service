using System;
using System.Collections.Generic;
using System.Text;

namespace br.com.fiap.cloudgames.Payment.Application.Events
{
    public class PaymentFailedEvent
    {
        public Guid EventId { get; init; }
        public Guid OrderId { get; set; }
    }
}
