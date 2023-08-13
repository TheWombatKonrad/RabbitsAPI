using WonderfulRabbitsApi.Authorization;
using WonderfulRabbitsApi.Models.Users;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using WonderfulRabbitsApi.Services.Interfaces;

namespace WonderfulRabbitsApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private IUserService _userService;
        private IMapper _mapper;

        public UsersController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = _mapper.Map<List<UserModel>>(await _userService.GetUsersAsync());
            return Ok(users);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = _mapper.Map<UserModel>(await _userService.GetUserAsync(id));
            return Ok(user);
        }

        // [HttpGet("current")]
        // public IActionResult GetCurrentUser()
        // {
        //     var user = _userService.GetCurrentUser();
        //     return Ok(user);
        // }

        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UpdateUserModel model)
        {
            await _userService.UpdateUserAsync(id, model);
            return Ok(new { message = "User updated successfully" });
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult AuthenticateUser(AuthenticateRequestModel model)
        {
            var response = _userService.AuthenticateUser(model);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUserAsync(RegisterUserModel model)
        {
            int id = await _userService.RegisterUserAsync(model);

            return Ok(new { message = "Registration successful", id = id });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserAsync(int id)
        {
            await _userService.DeleteUserAsync(id);
            return Ok(new { message = "User deleted successfully" });
        }

        // [HttpDelete("current")]
        // public IActionResult DeleteCurrentUser()
        // {
        //     _userService.DeleteCurrentUser();
        //     return Ok(new { message = "User deleted successfully" });
        // }

    }
}
