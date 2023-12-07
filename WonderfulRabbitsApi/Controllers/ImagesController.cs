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

        [HttpPost("upload")]
        public async Task<IActionResult> UploadImageAsync(UploadImageModel model)
        {
            int id = await _service.UploadImageAsync(model);

            return Ok(new { message = "Image successfully added", id });
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetImageAsync(int id)
        {
            var image = _mapper.Map<ImageModel>(await _service.GetImageAsync(id));

            return Ok(image);
        }

        [AllowAnonymous]
        [HttpGet()]
        public async Task<IActionResult> GetImagesAsync()
        {
            var models = await _service.GetImagesAsync();
            var images = _mapper.Map<List<ImageModel>>(models);

            return Ok(images);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateImageAsync(int id, UpdateImageModel model)
        {
            await _service.UpdateImageAsync(id, model);
            return Ok(new { message = "The image has been updated successfully." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteImageAsync(int id)
        {
            await _service.DeleteImageAsync(id);
            return Ok(new { message = "The image has been deleted successfully." });
        }
    }
}
