using Bloggie.Web.Models.Domain;
using Bloggie.Web.Models.ViewModels;
using Bloggie.Web.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Bloggie.Web.Controllers
{
    public class BlogController : Controller
    {
        private readonly IBlogPostRepository _blogPostRepositroy;
        private readonly IBlogPostLikeRepository _blogPostLikeRepositroy;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IBlogPostCommentRepository blogPostCommentRepository;

        public BlogController(IBlogPostRepository blogPostRepositroy, IBlogPostLikeRepository blogPostLikeRepositroy,SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, IBlogPostCommentRepository blogPostCommentRepository)
        {
            _blogPostRepositroy = blogPostRepositroy;
            _blogPostLikeRepositroy = blogPostLikeRepositroy;
            _signInManager = signInManager;
            _userManager = userManager;
            this.blogPostCommentRepository = blogPostCommentRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string urlHandle)
        {
            var liked = false;
            var blogPost = await _blogPostRepositroy.GetByUrlHandleAsync(urlHandle);
            var blogDetailsViewModel = new BlogPostDetailViewModel();

            if (blogPost != null)
            {
                var totalLikes = await _blogPostLikeRepositroy.GetTotalLikes(blogPost.Id);

                if(_signInManager.IsSignedIn(User))
                {
                    var likesForBlog = await _blogPostLikeRepositroy.GetLikesForBlog(blogPost.Id);

                    var userId = _userManager.GetUserId(User);

                    if (userId != null)
                    {
                        var likeFromUser = likesForBlog.FirstOrDefault(x => x.UserId == Guid.Parse(userId));
                        liked = likeFromUser != null;
                    }
                }

                var comments = await blogPostCommentRepository.GetCommentsByBlogIdAsync(blogPost.Id);

                var blogCommentsForView = new List<BlogComment>();

                foreach (var blogComment in comments)
                {
                    blogCommentsForView.Add(new BlogComment
                    {
                        Description = blogComment.Description,
                        DateAdded = blogComment.DateAdded,
                        Username = (await _userManager.FindByIdAsync(blogComment.UserId.ToString())).UserName
                    });
                }

                blogDetailsViewModel = new BlogPostDetailViewModel
                {
                    Id = blogPost.Id,
                    Content = blogPost.Content,
                    PageTitle = blogPost.PageTitle,
                    Author = blogPost.Author,
                    FeaturedImageUrl = blogPost.FeaturedImageUrl,
                    Heading = blogPost.Heading,
                    PublishedDate = blogPost.PublishedDate,
                    ShortDescription = blogPost.ShortDescription,
                    UrlHandle = blogPost.UrlHandle,
                    Visible = blogPost.Visible,
                    Tags = blogPost.Tags,
                    TotalLikes = totalLikes,
                    IsLiked = liked,
                    BlogComments = blogCommentsForView

                };
            }

            return View(blogDetailsViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Index(BlogPostDetailViewModel blogPostDetailVm)
        {
            if (_signInManager.IsSignedIn(User))
            {
                var domainModel = new BlogPostComment
                {
                    BlogPostId = blogPostDetailVm.Id,
                    Description = blogPostDetailVm.CommentDescription,
                    UserId = Guid.Parse(_userManager.GetUserId(User))
                };

                await blogPostCommentRepository.AddAsync(domainModel);

                return RedirectToAction("Index", "Blog", new {
                    urlHandle = blogPostDetailVm.UrlHandle
                });
            }

            return View();
        }
    }
}
