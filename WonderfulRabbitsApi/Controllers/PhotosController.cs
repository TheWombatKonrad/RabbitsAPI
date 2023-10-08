using WonderfulRabbitsApi.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using WonderfulRabbitsApi.Services.Interfaces;
using WonderfulRabbitsApi.Models.Rabbits;
using WonderfulRabbitsApi.Models.Photos;
using WonderfulRabbitsApi.Models;

namespace WonderfulRabbitsApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/photos")]
    public class PhotosController : ControllerBase
    {
        private IPhotoService _service;
        private IMapper _mapper;

        public PhotosController(IPhotoService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> RegisterPhoto(RegisterPhotoModel model)
        {
            int id = await _service.RegisterPhotoAsync(model);

            return Ok(new { message = "Photo successfully added", id });
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var photo = _mapper.Map<PhotoModel>(await _service.GetPhotoAsync(id));

            return Ok(photo);
        }

        [AllowAnonymous]
        [HttpGet()]
        public async Task<IActionResult> GetPhotos()
        {
            var photo = _mapper.Map<List<PhotoModel>>(await _service.GetPhotosAsync());

            return Ok(photo);
        }
    }
}
