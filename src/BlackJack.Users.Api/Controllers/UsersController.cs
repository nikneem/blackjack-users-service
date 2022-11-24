using BlackJack.Users.Functions.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;

namespace BlackJack.Users.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {

        [HttpGet]
        public  IActionResult Get()
        {
            var userDto = new UserDto
            {
                Id = Guid.NewGuid()
            };
            return Ok(userDto);
        }

        [HttpGet("{id:guid}")]
        public IActionResult Get(Guid id)
        {
            var userDto = new UserDto
            {
                Id = id
            };
            return Ok(userDto);
        }

    }
}
