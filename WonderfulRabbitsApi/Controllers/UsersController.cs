using WonderfulRabbitsApi.Authorization;
using WonderfulRabbitsApi.Models.Users;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using WonderfulRabbitsApi.Helpers;
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
        public IActionResult GetUsers()
        {
            var users = _mapper.Map<List<UserModel>>(_userService.GetUsers());
            return Ok(users);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public IActionResult GetUser(int id)
        {
            var user = _mapper.Map<UserModel>(_userService.GetUser(id));
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
        public IActionResult UpdateUser(int id, UpdateUser model)
        {
            _userService.UpdateUser(id, model);
            return Ok(new { message = "User updated successfully" });
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult AuthenticateUser(AuthenticateRequest model)
        {
            var response = _userService.AuthenticateUser(model);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUserAsync(RegisterUserModel model)
        {
            int? id = await _userService.RegisterUserAsync(model);

            return Ok(new { message = "Registration successful", id = id });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            _userService.DeleteUser(id);
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
