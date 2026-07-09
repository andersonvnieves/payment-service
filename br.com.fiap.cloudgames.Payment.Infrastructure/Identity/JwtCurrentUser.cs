using br.com.fiap.cloudgames.Payment.Application.Abstractions;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;

namespace br.com.fiap.cloudgames.Payment.Infrastructure.Identity
{
    public class JwtCurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public JwtCurrentUser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid UserId =>
            Guid.Parse(_httpContextAccessor.HttpContext!.User
                .FindFirst(JwtRegisteredClaimNames.Sub)!.Value);

        public string Name =>
            _httpContextAccessor.HttpContext!.User
                .FindFirst(JwtRegisteredClaimNames.Name)!.Value;

        public string Email =>
            _httpContextAccessor.HttpContext!.User
                .FindFirst(JwtRegisteredClaimNames.Email)!.Value;
    }
}
