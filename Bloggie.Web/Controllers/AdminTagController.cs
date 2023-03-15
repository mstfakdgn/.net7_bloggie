using Azure.Core;
using Bloggie.Web.Data;
using Bloggie.Web.Models.Domain;
using Bloggie.Web.Models.ViewModels;
using Bloggie.Web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bloggie.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminTagController : Controller
    {
        private ITagRepository _tagRepositroy;
        public AdminTagController(ITagRepository tagRepositroy)
        {
            _tagRepositroy = tagRepositroy;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles ="Admin")]
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ActionName("Add")]
        public async Task<IActionResult> Add(AddTagRequest request)
        {
            //var name = Request.Form["name"];
            //var displayName = Request.Form["displayName"];

            var tag = new Tag
            {
                Name = request.Name,
                DisplayName = request.DisplayName,
            };

            await _tagRepositroy.AddAsync(tag);

            return RedirectToAction("List");
        }

        [HttpGet]
        [ActionName("List")]
        public async Task<IActionResult> List()
        {
            var tags = await _tagRepositroy.GetAllAsync();
            return View(tags);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            //var tag = _bloggieDBContext.Tags.Find(id);

            var tag = await _tagRepositroy.GetByIdAsync(id);

            if (tag != null)
            {
                var editTagRequest = new EditTagRequest
                {
                    Id = tag.Id,
                    Name = tag.Name,
                    DisplayName = tag.DisplayName,
                };

                return View(editTagRequest);
            }

            return View(null);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditTagRequest request)
        {
            Tag tag = new()
            {
                Id = request.Id,
                Name = request.Name,
                DisplayName = request.DisplayName
            };

            var editedTag =  await _tagRepositroy.UpdateAsync(tag);

            if (editedTag != null)
            {
                // Show success notification
                return RedirectToAction("Edit", new { id = request.Id });
            }

            // Show failure notification
            return RedirectToAction("Edit", new { id = request.Id});

        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {

            var deleteResult = await _tagRepositroy.DeleteAsync(id);

            if (deleteResult != null)
            {
                // Show success notification
                return RedirectToAction("List");
            }

            // Show error notification
            return RedirectToAction("Edit", new { id = id });

        }
    }
}
