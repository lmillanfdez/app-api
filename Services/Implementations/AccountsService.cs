using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;

public class AccountsService : IAccountsService
{
    private readonly IOptions<ConfigurationsDTO> _configs;

    public AccountsService(IOptions<ConfigurationsDTO> configs)
    {
        _configs = configs;
    }

    public string CreateToken(User user)
    {
        var encodedKey = Encoding.ASCII.GetBytes(_configs.Value.JwtSettings.SecretKey);
        var signinKey = new SymmetricSecurityKey(encodedKey);
        var signinCredentials = new SigningCredentials(signinKey, SecurityAlgorithms.HmacSha256Signature);

        var claims = new []
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _configs.Value.JwtSettings.Issuer,
            Subject = new ClaimsIdentity(claims),
            SigningCredentials = signinCredentials,
            Expires = DateTime.Now.AddMinutes(_configs.Value.JwtSettings.TokenLifeTime)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}