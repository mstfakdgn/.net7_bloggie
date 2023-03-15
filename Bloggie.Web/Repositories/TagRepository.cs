using Azure.Core;
using Bloggie.Web.Data;
using Bloggie.Web.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bloggie.Web.Repositories
{
    public class TagRepository : ITagRepository
    {
        private BloggieDBContext _bloggieDBContext;

        public TagRepository(BloggieDBContext bloggieDBContext)
        {
            _bloggieDBContext = bloggieDBContext;
        }

        public async Task<Tag> AddAsync(Tag tag)
        {
            await _bloggieDBContext.Tags.AddAsync(tag);
            await _bloggieDBContext.SaveChangesAsync();

            return tag;
        }

        public async Task<Tag?> DeleteAsync(Guid id)
        {
            var tag = await _bloggieDBContext.Tags.FindAsync(id);

            if (tag != null)
            {
                _bloggieDBContext.Tags.Remove(tag);
                await _bloggieDBContext.SaveChangesAsync();

                return tag;
            }

            return null;
        }

        public async Task<IEnumerable<Tag>> GetAllAsync()
        {
            var tags = await _bloggieDBContext.Tags.ToListAsync();

            return tags;
        }

        public async Task<Tag?> GetByIdAsync(Guid id)
        {
            var tag = await _bloggieDBContext.Tags.FirstOrDefaultAsync(t => t.Id == id);

            return tag;
        }

        public async Task<Tag?> UpdateAsync(Tag tag)
        {
            var existingTag = await _bloggieDBContext.Tags.FindAsync(tag.Id);

            if (existingTag != null)
            {
                existingTag.Name = tag.Name;
                existingTag.DisplayName = tag.DisplayName;

                await _bloggieDBContext.SaveChangesAsync();

                return existingTag;
               
            }

            return null;
        }
    }
}
