using WonderfulRabbitsApi.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using WonderfulRabbitsApi.Services.Interfaces;
using WonderfulRabbitsApi.Models.Rabbits;

namespace WonderfulRabbitsApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/rabbits")]
    public class RabbitsController : ControllerBase
    {
        private IRabbitService _service;
        private IMapper _mapper;

        public RabbitsController(IRabbitService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetRabbits()
        {
            var users = _mapper.Map<List<RabbitModel>>(await _service.GetRabbitsAsync());
            return Ok(users);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRabbit(int id)
        {
            var user = _mapper.Map<RabbitModel>(await _service.GetRabbitAsync(id));
            return Ok(user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRabbit(int id, UpdateRabbitModel model)
        {
            await _service.UpdateRabbitAsync(id, model);
            return Ok(new { message = "Rabbit updated successfully" });
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterRabbitAsync(RegisterRabbitModel model)
        {
            int id = await _service.RegisterRabbitAsync(model);

            return Ok(new { message = "Registration successful", id });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRabbitAsync(int id)
        {
            await _service.DeleteRabbitAsync(id);
            return Ok(new { message = "Rabbit deleted successfully" });
        }
    }
}
