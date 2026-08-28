using System;
using System.Collections.Generic;
using System.Text;

namespace br.com.fiap.cloudgames.Payment.Infrastructure.Config
{
    public class AwsSqsSettings
    {
        public string PaymentProcessedQueueUrl { get; set; }
    }
}
