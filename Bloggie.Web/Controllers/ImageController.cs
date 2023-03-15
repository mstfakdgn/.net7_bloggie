using Bloggie.Web.Repositories;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Bloggie.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        public IImageRepository imageRepository;
        public ImageController(IImageRepository imageRepository)
        {
            this.imageRepository = imageRepository;
        }

        [HttpPost]
        public async Task<IActionResult> UploadAsync(IFormFile file)
        {
            
            var response = await imageRepository.UploadAsync(file);

            if (response != null)
            {
                return new JsonResult(new { link = response });
            }

            return Problem("Something went wrong", null, (int)HttpStatusCode.InternalServerError);

        }
    }
}
