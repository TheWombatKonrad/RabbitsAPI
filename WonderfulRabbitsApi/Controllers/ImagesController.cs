using WonderfulRabbitsApi.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using WonderfulRabbitsApi.Services.Interfaces;
using WonderfulRabbitsApi.Models.Images;
using WonderfulRabbitsApi.Models;

namespace WonderfulRabbitsApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/images")]
    public class ImagesController : ControllerBase
    {
        private IImageService _service;
        private IMapper _mapper;

        public ImagesController(IImageService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [AllowAnonymous] //TODO: remove allowanonymous
        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage(UploadImageModel model)
        {
            int id = await _service.UploadImageAsync(model);

            return Ok(new { message = "Image successfully added", id });
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetImage(int id)
        {
            var image = _mapper.Map<ImageModel>(await _service.GetImageAsync(id));

            return Ok(image);
        }

        [AllowAnonymous]
        [HttpGet()]
        public async Task<IActionResult> GetImages()
        {
            var models = await _service.GetImagesAsync();
            var images = _mapper.Map<List<ImageModel>>(models);

            for (int i = 0; i < images.Count(); i++)
            {
                images[i].Base64ImageData = Convert.ToBase64String(models[i].ImageData);
            }

            return Ok(images);
        }
    }
}
