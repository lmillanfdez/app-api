using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[ApiController]
[Produces("application/json")]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly SignInManager<IdentityUser> _signInManager;

    private readonly IAuthService _authService;

    private readonly IBaseRepository<RefreshToken> _refreshTokenRepository;

    public AuthController(SignInManager<IdentityUser> signInManager,
                        IAuthService authService,
                        IBaseRepository<RefreshToken> refreshTokenRepository)
    {
        _signInManager = signInManager;
        _authService = authService;
        _refreshTokenRepository = refreshTokenRepository;
    }

    [AllowAnonymous]
    [HttpPost("signup")]
    public async Task<IActionResult> SignUp([FromBody] SignUpRequestDTO request)
    {
        if(!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var result = await _authService.SignUp(request);
            return Ok(result);
        }
        catch(AppException appExc)
        {
            return new BadRequestObjectResult(appExc.Message);
        }
        catch(Exception exc)
        {
            return new ObjectResult(exc.Message){StatusCode = (int)HttpStatusCode.InternalServerError};
        }
    }

    [AllowAnonymous]
    [HttpPost("signin")]
    public async Task<IActionResult> SignIn([FromBody] SignInRequestDTO request)
    {
        if(!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var result = await _authService.SignIn(request);
            
            if(result != null)
                return Ok(result);

            return Unauthorized();
        }
        catch(Exception exc)
        {
            return new ObjectResult(exc.Message){StatusCode = (int)HttpStatusCode.InternalServerError};
        }
    }

    [AllowAnonymous]
    [HttpGet("refreshAccessToken/{refreshTokenValue}")]
    public async Task<IActionResult> RefreshAccessToken([FromRoute] string refreshTokenValue)
    {
        try
        {
            var newAccessToken = await _authService.GetNewAccessToken(refreshTokenValue);

            return Ok(newAccessToken);
        }
        catch(AppException appExc)
        {
            return new ObjectResult(appExc.Message){StatusCode = (int)HttpStatusCode.Unauthorized};
        }
        catch (Exception exc)
        {
            return new ObjectResult(exc.Message){StatusCode = (int)HttpStatusCode.InternalServerError};
        }
    }

    [HttpPost("signout")]
    public async Task<IActionResult> SignOut()
    {
        try
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }
        catch(Exception exc)
        {
            return new ObjectResult(exc.Message){StatusCode = (int)HttpStatusCode.InternalServerError};
        }
    }

    [HttpDelete("revokeRefreshToken/{refreshTokenValue}")]
    public async Task<IActionResult> RevokeRefreshToken([FromRoute] string refreshTokenValue)
    {
        try
        {
            var refreshToken = await _refreshTokenRepository.Delete(item => item.RefreshTokenValue == refreshTokenValue);

            if(refreshToken != null)
                return Ok(new {RefreshToken = refreshTokenValue});

            return NotFound("Invalid refresh token");
        }
        catch (Exception exc)
        {
            return new ObjectResult(exc.Message){StatusCode = (int)HttpStatusCode.InternalServerError};
        }
    }
}