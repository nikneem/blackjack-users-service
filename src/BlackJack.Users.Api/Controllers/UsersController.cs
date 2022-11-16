using BlackJack.Users.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlackJack.Users.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IBlackJackUsersService _service;

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var userDto = await _service.Create();
        return Ok(userDto);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var userDto = await _service.Restore(id);
        return userDto == null ? BadRequest() : Ok(userDto);
    }

    public UsersController(IBlackJackUsersService service)
    {
        _service = service;
    }
}