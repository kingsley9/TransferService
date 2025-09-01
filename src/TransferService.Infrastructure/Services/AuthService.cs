using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TransferService.Application.Interfaces;

namespace TransferService.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _config;
        private readonly ICustomerService _customerService;

        public AuthService(IConfiguration config, ICustomerService customerService)
        {
            _config = config;
            _customerService = customerService;
        }

        public async Task<string?> AuthenticateAsync(string username, string password)
        {
            var isValid = await _customerService.AuthenticateAsync(username, password);
            if (!isValid)
                return null;

            var customerDto = await _customerService.GetCustomerByUsernameAsync(username);
            if (customerDto == null)
                return null;

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim("full_name", customerDto.FullName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("customer_id", customerDto.Id.ToString()),
                new Claim(ClaimTypes.Role, "User"),
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _config["Jwt:Key"] ?? "HNVaGGrmsD0cuBel15RBCssL/w38o7SPFVGgcUE+jmE="
                )
            );
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
