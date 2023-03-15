using Bloggie.Web.Models.Domain;

namespace Bloggie.Web.Repositories
{
    public interface IBlogPostRepository
    { 
        Task<IEnumerable<BlogPost>> GetAllAsync();
        
        Task<BlogPost?> GetByIdAsync(Guid id);
        Task<BlogPost?> GetByUrlHandleAsync(string urlHandle);
        Task<BlogPost> AddAsync(BlogPost BlogPost);
        Task<BlogPost?> UpdateAsync(BlogPost BlogPost);
        Task<BlogPost?> DeleteAsync(Guid id);
    }
}
