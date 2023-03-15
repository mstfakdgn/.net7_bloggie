using Bloggie.Web.Data;
using Bloggie.Web.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bloggie.Web.Repositories
{
    public class BlogPostLikeRepository : IBlogPostLikeRepository
    {

        private readonly BloggieDBContext _bloggieDBContext;
        public BlogPostLikeRepository(BloggieDBContext bloggieDBContext)
        {
            _bloggieDBContext = bloggieDBContext;
        }

        public async Task<BlogPostLike> AddLikeForBlog(BlogPostLike blogPostLike)
        {
            await _bloggieDBContext.BlogPostLikes.AddAsync(blogPostLike);
            await _bloggieDBContext.SaveChangesAsync();

            return blogPostLike;
        }

        public async Task<IEnumerable<BlogPostLike>> GetLikesForBlog(Guid blogPostId)
        {
            return await _bloggieDBContext.BlogPostLikes.Where(x => x.BlogPostId == blogPostId).ToListAsync();
        }

        public async Task<int> GetTotalLikes(Guid blogPostId)
        {
            return await _bloggieDBContext.BlogPostLikes.CountAsync(x => x.BlogPostId == blogPostId);
        }
    }
}
