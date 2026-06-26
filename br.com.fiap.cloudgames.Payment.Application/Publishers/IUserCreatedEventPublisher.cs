using br.com.fiap.cloudgames.Payment.Application.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace br.com.fiap.cloudgames.Payment.Application.Publishers
{
    public interface IUserCreatedEventPublisher
    {
        Task PublishAsync(UserCreatedEvent message);
    }
}
