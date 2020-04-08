using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

public class AuthService : IAuthService
{
    private readonly IOptions<ConfigurationsDTO> _configs;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;

    private readonly IBaseRepository<User> _userRepository;
    private readonly IBaseRepository<RefreshToken> _refreshTokenRepository;

    public AuthService(IOptions<ConfigurationsDTO> configs,
                        SignInManager<IdentityUser> signInManager,
                        UserManager<IdentityUser> userManager,
                        IBaseRepository<User> userRepository,
                        IBaseRepository<RefreshToken> refreshTokenRepository)
    {
        _configs = configs;
        _signInManager = signInManager;
        _userManager = userManager;
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<SignUpResponseDTO> SignUp(SignUpRequestDTO request)
    {
        var identityUser = new IdentityUser 
        { 
            Email = request.Email,
            UserName = request.Email
        };
        var result = await _userManager.CreateAsync(identityUser, request.Password);

        if(result.Succeeded)
        {
            var user = new User
            {
                Guid = identityUser.Id,
                EmailAddress = request.Email,
                Username = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                CreatedOnUtc = DateTime.UtcNow,
                Active = true
            };
            await _userRepository.Add(user);
            
            return new SignUpResponseDTO{UserId = user.Guid, Email = user.EmailAddress};
        }

        var exceptionMessage = String.Join('\n', result.Errors.Select(item => item.Description));
        throw new AppException(exceptionMessage);
    }

    public async Task<SignInResponseDTO> SignIn(SignInRequestDTO request)
    {
        var result = await _signInManager.PasswordSignInAsync(request.Email, request.Password, false, false);

        if(result.Succeeded)
        {
            var user = await _userRepository.Get(item => item.EmailAddress == request.Email);
            var accessToken = CreateAccessToken(user);
            var refreshToken = await CreateRefreshToken(user);

            return new SignInResponseDTO 
            { 
                UserId = user.Guid, 
                AccessToken = accessToken, 
                RefreshToken = refreshToken.RefreshTokenValue 
            };
        }

        return null;
    }

    private string CreateAccessToken(User user)
    {
        var encodedKey = Encoding.ASCII.GetBytes(_configs.Value.JwtSettings.SecretKey);
        var signinKey = new SymmetricSecurityKey(encodedKey);
        var signinCredentials = new SigningCredentials(signinKey, SecurityAlgorithms.HmacSha256Signature);

        var claims = new []
        {
            new Claim(ClaimTypes.NameIdentifier, user.Guid.ToString()),
            new Claim(ClaimTypes.Email, user.EmailAddress)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _configs.Value.JwtSettings.Issuer,
            Subject = new ClaimsIdentity(claims),
            SigningCredentials = signinCredentials,
            Expires = DateTime.UtcNow.AddMinutes(_configs.Value.JwtSettings.AccessTokenLifeTime),
            NotBefore = DateTime.UtcNow
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    private async Task<RefreshToken> CreateRefreshToken(User user, int refreshTokenSize = 32)
    {
        var cryptofiedBytes = new byte[refreshTokenSize];

        using(var randomGenerator = RandomNumberGenerator.Create())
        {
            randomGenerator.GetBytes(cryptofiedBytes);
        }

        var refreshTokenValue =  Convert.ToBase64String(cryptofiedBytes);
        var refreshToken = await _refreshTokenRepository.Get(item => item.UserId == user.Guid);
        
        var dateTimeUtcNow = DateTime.UtcNow;

        if(refreshToken != null)
        {
            refreshToken.RefreshTokenValue = refreshTokenValue;
            refreshToken.CreatedOnUtc = dateTimeUtcNow;
            refreshToken.ExpiresOnUtc = dateTimeUtcNow.AddSeconds(_configs.Value.JwtSettings.RefreshTokenLifeTime);

            await _refreshTokenRepository.Update(refreshToken);
        }
        else
        {
            refreshToken = new RefreshToken
            {
                UserId = user.Guid,
                RefreshTokenValue = refreshTokenValue,
                CreatedOnUtc = dateTimeUtcNow,
                ExpiresOnUtc = dateTimeUtcNow.AddSeconds(_configs.Value.JwtSettings.RefreshTokenLifeTime)
            };

            await _refreshTokenRepository.Add(refreshToken);
        }

        return refreshToken;
    }

    public async Task<NewAccessTokenDTO> GetNewAccessToken(string refreshTokenValue)
    {
        var refreshToken = await _refreshTokenRepository.Get(item => item.RefreshTokenValue == refreshTokenValue
                                                            && DateTime.UtcNow < item.ExpiresOnUtc);

        if(refreshToken != null)
        {
            var user = await _userRepository.Get(item => item.Guid == refreshToken.UserId
                                                        && item.Active && !item.Deleted);

            if(user != null)
            {
                var accessToken = CreateAccessToken(user);
                return new NewAccessTokenDTO{ UserId = user.Guid, AccessToken = accessToken };
            }
            else
            {
                throw new AppException("Invalid user");
            }
        }
        else
        {
            throw new AppException("Invalid refresh token");
        }
    }
}