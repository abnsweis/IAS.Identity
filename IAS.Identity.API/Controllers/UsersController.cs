using IAS.Identity.Application.Common.Dtos.Users;
using IAS.Identity.Application.Common.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IAS.Identity.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            this._userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaginatedListUsers(
            int page = 1,
            int pageSize = 10)
        {
            var paginatedUsers = await _userService.GetPaginatedListUsersAsync(page, pageSize);
            return Ok(paginatedUsers);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto createUser)
        {
            var createdUser = await _userService.CreateUserAsync(createUser);
            return CreatedAtAction(nameof(GetUserById), new { userId = createdUser.Id }, createdUser);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(Guid userId)
        {
            var user = await _userService.GetUserById(userId);
            return Ok(user);
        }
    }
}