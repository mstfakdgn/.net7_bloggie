using Azure;
using Azure.Core;
using Bloggie.Web.Data;
using Bloggie.Web.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bloggie.Web.Repositories
{
    public class BlogPostRepository : IBlogPostRepository
    {
        private BloggieDBContext _bloggieDBContext;

        public BlogPostRepository(BloggieDBContext bloggieDBContext)
        {
            _bloggieDBContext = bloggieDBContext;
        }

        public async Task<BlogPost> AddAsync(BlogPost blogPost)
        {
            await _bloggieDBContext.BlogPosts.AddAsync(blogPost);
            await _bloggieDBContext.SaveChangesAsync();

            return blogPost;
        }

        public async Task<BlogPost?> DeleteAsync(Guid id)
        {
            var blogPost = await _bloggieDBContext.BlogPosts.FindAsync(id);

            if (blogPost != null)
            {
                _bloggieDBContext.BlogPosts.Remove(blogPost);
                await _bloggieDBContext.SaveChangesAsync();

                return blogPost;
            }

            return null;
        }

        public async Task<IEnumerable<BlogPost>> GetAllAsync()
        {
            var blogPosts = await _bloggieDBContext.BlogPosts.Include(x => x.Tags).ToListAsync();

            return blogPosts;
        }

        public async Task<BlogPost?> GetByIdAsync(Guid id)
        {
            var blogPost = await _bloggieDBContext.BlogPosts.Include(x => x.Tags).FirstOrDefaultAsync(t => t.Id == id);

            return blogPost;
        }

        public async Task<BlogPost?> GetByUrlHandleAsync(string urlHandle)
        {
            return await _bloggieDBContext.BlogPosts.Include(x => x.Tags).FirstOrDefaultAsync(t => t.UrlHandle == urlHandle);
        }

        public async Task<BlogPost?> UpdateAsync(BlogPost blogPost)
        {
            var existingBlogPost = await _bloggieDBContext.BlogPosts.FindAsync(blogPost.Id);

            if (existingBlogPost != null)
            {
                existingBlogPost.Heading            =blogPost.Heading;
                existingBlogPost.PageTitle          =blogPost.PageTitle;
                existingBlogPost.Content            =blogPost.Content;
                existingBlogPost.ShortDescription   =blogPost.ShortDescription;
                existingBlogPost.FeaturedImageUrl   =blogPost.FeaturedImageUrl;
                existingBlogPost.UrlHandle          =blogPost.UrlHandle;
                existingBlogPost.PublishedDate      =blogPost.PublishedDate;
                existingBlogPost.Author             =blogPost.Author;
                existingBlogPost.Visible            =blogPost.Visible;

                existingBlogPost.Tags = blogPost.Tags;

                await _bloggieDBContext.SaveChangesAsync();

                return existingBlogPost;

            }

            return null;
        }
    }
}
