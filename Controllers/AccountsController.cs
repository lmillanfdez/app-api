using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

[Authorize]
[ApiController]
[Produces("application/json")]
[Route("accounts")]
public class AccountsController : ControllerBase
{
    private readonly IAccountsService _accountsService;

    private readonly IBaseRepository<User> _userRepository;

    public AccountsController(IAccountsService accountsService, IBaseRepository<User> userRepository)
    {
        _accountsService = accountsService;
        _userRepository = userRepository;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser([FromRoute] string id)
    {
        try
        {
            var user = await _userRepository.Get(item => item.Guid == id);

            if(user != null)
                return Ok(new { UserId = user.Guid, FirstName = user.FirstName, LastName = user.LastName });

            return NotFound();
        }
        catch (Exception exc)
        {
            return new ObjectResult(exc.Message){StatusCode = (int)HttpStatusCode.InternalServerError};
        }
    }

    [HttpPut]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequestDTO request)
    {
        if(!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var result = await _accountsService.UpdateUser(request);

            if(result != null)
                return Ok(result);

            return NotFound("User does not exist");
        }
        catch(Exception exc)
        {
            return new ObjectResult(exc.Message){StatusCode = (int)HttpStatusCode.InternalServerError};
        }
    }
}