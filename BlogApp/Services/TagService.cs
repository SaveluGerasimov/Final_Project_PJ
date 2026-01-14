using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BlogApp.Data;
using BlogApp.Models;

namespace BlogApp.Services
{
    public interface ITagService
    {
        Task<Tag> CreateTagAsync(string name);
        Task<IEnumerable<Tag>> GetAllTagsAsync();
        Task<Tag> GetTagByIdAsync(int id);
        Task<Tag> UpdateTagAsync(int id, string name);
        Task<bool> DeleteTagAsync(int id);
    }
    
    public class TagService : ITagService
    {
        private readonly ApplicationDbContext _context;
        
        public TagService(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<Tag> CreateTagAsync(string name)
        {
            // Check if tag already exists
            var existingTag = await _context.Tags
                .FirstOrDefaultAsync(t => t.Name.ToLower() == name.ToLower());
            
            if (existingTag != null)
            {
                return existingTag;
            }
            
            var tag = new Tag { Name = name };
            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();
            return tag;
        }
        
        public async Task<IEnumerable<Tag>> GetAllTagsAsync()
        {
            return await _context.Tags
                .OrderBy(t => t.Name)
                .ToListAsync();
        }
        
        public async Task<Tag> GetTagByIdAsync(int id)
        {
            return await _context.Tags.FindAsync(id);
        }
        
        public async Task<Tag> UpdateTagAsync(int id, string name)
        {
            var tag = await _context.Tags.FindAsync(id);
            if (tag == null)
            {
                throw new Exception("Tag not found");
            }
            
            tag.Name = name;
            await _context.SaveChangesAsync();
            return tag;
        }
        
        public async Task<bool> DeleteTagAsync(int id)
        {
            var tag = await _context.Tags.FindAsync(id);
            if (tag == null)
            {
                return false;
            }
            
            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}