using WonderfulRabbitsApi.Authorization;
using WonderfulRabbitsApi.Models.Users;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using WonderfulRabbitsApi.Services.Interfaces;
using WonderfulRabbitsApi.Models.Rabbits;
using System.Text.Json;
using WonderfulRabbitsApi.Helpers;

namespace WonderfulRabbitsApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private IUserService _service;
        private IMapper _mapper;

        public UsersController(IUserService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = _mapper.Map<List<UserModel>>(await _service.GetUsersAsync());
            return Ok(users);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = _mapper.Map<UserModel>(await _service.GetUserAsync(id));

            return Ok(user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UpdateUserModel model)
        {
            await _service.UpdateUserAsync(id, model);
            return Ok(new { message = "User updated successfully" });
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult AuthenticateUser(AuthenticateRequestModel model)
        {
            var response = _service.AuthenticateUser(model);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUserAsync(RegisterUserModel model)
        {
            int id = await _service.RegisterUserAsync(model);

            return Ok(new { message = "Registration successful", id });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserAsync(int id)
        {
            await _service.DeleteUserAsync(id);
            return Ok(new { message = "User deleted successfully" });
        }
    }
}
