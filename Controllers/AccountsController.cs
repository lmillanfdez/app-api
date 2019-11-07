using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

[AllowAnonymous]
[Route("accounts")]
public class AccountsController : ControllerBase
{
    private readonly IAccountsService _accountsService;
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;

    public AccountsController(IAccountsService accountsService, SignInManager<User> signInManager,
                                UserManager<User> userManager)
    {
        _accountsService = accountsService;
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [HttpPost("signup")]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequestDTO request)
    {
        if(!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = new User { Email = request.Email, UserName = request.Email };
        var result = await _userManager.CreateAsync(user, request.Password);

        if(result.Succeeded)
            return Ok();

        return BadRequest(String.Join('\n', result.Errors.Select(item => item.Description)));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserRequestDTO request)
    {
        if(!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var result = await _signInManager.PasswordSignInAsync(request.Email, request.Password, false, false);

            if(result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                var token = _accountsService.CreateToken(user);

                return Ok(new { UserId = user.Id, Token = token });
            }
            
            return Unauthorized();
        }
        catch(Exception exc)
        {
            return BadRequest(exc.Message);
        }
    }

    [HttpPut]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequestDTO request)
    {
        return Ok();
    }

    [HttpPost("refreshToken")]
    public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
    {
        return Ok();
    }
}