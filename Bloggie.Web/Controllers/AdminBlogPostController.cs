using Azure.Core;
using Bloggie.Web.Models.Domain;
using Bloggie.Web.Models.ViewModels;
using Bloggie.Web.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bloggie.Web.Controllers
{
    public class AdminBlogPostController : Controller
    {
        private ITagRepository _tagRepositroy;
        private IBlogPostRepository _blogPostRepositroy;

        public AdminBlogPostController(ITagRepository tagRepositroy, IBlogPostRepository blogPostRepositroy)
        {
            _tagRepositroy = tagRepositroy;
            _blogPostRepositroy = blogPostRepositroy;   
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var tags = await _tagRepositroy.GetAllAsync();

            var model = new AddBlogPostRequest
            {
                Tags = tags.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
            };

            return View(model);
        }

        [HttpPost]
        [ActionName("Add")]
        public async Task<IActionResult> Add(AddBlogPostRequest request)
        {
            var blogPost = new BlogPost
            {
                Heading = request.Heading,
                PageTitle = request.PageTitle,
                Content = request.Content,
                ShortDescription = request.ShortDescription,
                FeaturedImageUrl = request.FeaturedImageUrl,
                UrlHandle = request.UrlHandle,
                PublishedDate = request.PublishedDate,
                Author = request.Author,
                Visible = request.Visible
            };

            var selectedTags = new List<Tag>();
            foreach(var selectedTagId in request.SelectedTags)
            {
                var selectedTagIdAsGuid = Guid.Parse(selectedTagId);
                var existingTag = await _tagRepositroy.GetByIdAsync(selectedTagIdAsGuid);

                if (existingTag != null)
                {
                    selectedTags.Add(existingTag);
                }
            }
            blogPost.Tags = selectedTags;

            await _blogPostRepositroy.AddAsync(blogPost);

            return RedirectToAction("List");
        }

        [HttpGet]
        [ActionName("List")]
        public async Task<IActionResult> List()
        {
            var blogPosts = await _blogPostRepositroy.GetAllAsync();
            return View(blogPosts);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            //var tag = _bloggieDBContext.Tags.Find(id);

            var blogPost = await _blogPostRepositroy.GetByIdAsync(id);
            var tags = await _tagRepositroy.GetAllAsync();

            if (blogPost != null)
            {
                var editBlogPostRequest = new EditBlogPostRequest
                {
                    Id = blogPost.Id,
                    Heading = blogPost.Heading,
                    PageTitle = blogPost.PageTitle,
                    Content = blogPost.Content,
                    ShortDescription = blogPost.ShortDescription,
                    FeaturedImageUrl = blogPost.FeaturedImageUrl,
                    UrlHandle = blogPost.UrlHandle,
                    PublishedDate = blogPost.PublishedDate,
                    Author = blogPost.Author,
                    Visible = blogPost.Visible,
                    Tags = tags.Select(x => new SelectListItem { Text = x.Name, Value=x.Id.ToString()}),
                    SelectedTags = blogPost.Tags.Select(x => x.Id.ToString()).ToArray()
                };

                return View(editBlogPostRequest);
            }

            return View(null);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditBlogPostRequest request)
        {
            BlogPost blogPost = new BlogPost
            {
                Id = request.Id,
                Heading = request.Heading,
                PageTitle = request.PageTitle,
                Content = request.Content,
                ShortDescription = request.ShortDescription,
                FeaturedImageUrl = request.FeaturedImageUrl,
                UrlHandle = request.UrlHandle,
                PublishedDate = request.PublishedDate,
                Author = request.Author,
                Visible = request.Visible,

            };

            var selectedTags = new List<Tag>();
            foreach (var selectedTagId in request.SelectedTags)
            {
                if (Guid.TryParse(selectedTagId, out var tag))
                {
                    var foundTag = await _tagRepositroy.GetByIdAsync(tag);

                    if (foundTag != null)
                    {
                        selectedTags.Add(foundTag);
                    }
                }
            }
            blogPost.Tags = selectedTags;

            var editedBlogPost = await _blogPostRepositroy.UpdateAsync(blogPost);

            if (editedBlogPost != null)
            {
                // Show success notification
                return RedirectToAction("Edit", new { id = request.Id });
            }

            // Show failure notification
            return RedirectToAction("Edit", new { id = request.Id });

        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleteResult = await _blogPostRepositroy.DeleteAsync(id);

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
