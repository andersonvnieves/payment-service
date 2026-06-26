using br.com.fiap.cloudgames.Payment.Domain.Aggregates;

namespace br.com.fiap.cloudgames.Payment.Application.Services;

public interface ITokenService
{
    Task<string> GenerateTokenAsync(User user); 
}