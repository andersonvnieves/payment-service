using System;
using System.Collections.Generic;
using System.Text;

namespace br.com.fiap.cloudgames.Payment.Application.Publishers
{
    public interface IMessagePublisher
    {
        Task PublishAsync<T>(T message);
    }
}
