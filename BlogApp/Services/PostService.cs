using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BlogApp.Data;
using BlogApp.Models;
using BlogApp.DTOs;

namespace BlogApp.Services
{
    public interface IPostService
    {
        Task<Post> CreatePostAsync(CreatePostDto postDto, int authorId);
        Task<IEnumerable<Post>> GetAllPostsAsync();
        Task<IEnumerable<Post>> GetPostsByAuthorAsync(int authorId);
        Task<Post> GetPostByIdAsync(int id);
        Task<Post> UpdatePostAsync(int id, UpdatePostDto postDto, int userId, string userRole);
        Task<bool> DeletePostAsync(int id, int userId, string userRole);
        Task<IEnumerable<Post>> SearchPostsAsync(string searchTerm);
        Task<IEnumerable<Post>> GetPostsByTagAsync(int tagId);
    }
    
    public class PostService : IPostService
    {
        private readonly ApplicationDbContext _context;
        
        public PostService(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<Post> CreatePostAsync(CreatePostDto postDto, int authorId)
        {
            var post = new Post
            {
                Title = postDto.Title,
                Content = postDto.Content,
                AuthorId = authorId,
                CreatedAt = DateTime.UtcNow
            };
            
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();
            
            // Add tags if provided
            if (postDto.TagIds != null && postDto.TagIds.Any())
            {
                foreach (var tagId in postDto.TagIds)
                {
                    var tag = await _context.Tags.FindAsync(tagId);
                    if (tag != null)
                    {
                        _context.PostTags.Add(new PostTag
                        {
                            PostId = post.Id,
                            TagId = tagId
                        });
                    }
                }
                await _context.SaveChangesAsync();
            }
            
            return await GetPostByIdAsync(post.Id);
        }
        
        public async Task<IEnumerable<Post>> GetAllPostsAsync()
        {
            return await _context.Posts
                .Include(p => p.Author)
                .Include(p => p.PostTags)
                .ThenInclude(pt => pt.Tag)
                .Include(p => p.Comments)
                .ThenInclude(c => c.Author)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }
        
        public async Task<IEnumerable<Post>> GetPostsByAuthorAsync(int authorId)
        {
            return await _context.Posts
                .Where(p => p.AuthorId == authorId)
                .Include(p => p.Author)
                .Include(p => p.PostTags)
                .ThenInclude(pt => pt.Tag)
                .Include(p => p.Comments)
                .ThenInclude(c => c.Author)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }
        
        public async Task<Post> GetPostByIdAsync(int id)
        {
            return await _context.Posts
                .Include(p => p.Author)
                .Include(p => p.PostTags)
                .ThenInclude(pt => pt.Tag)
                .Include(p => p.Comments)
                .ThenInclude(c => c.Author)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
        
        public async Task<Post> UpdatePostAsync(int id, UpdatePostDto postDto, int userId, string userRole)
        {
            var post = await _context.Posts
                .Include(p => p.PostTags)
                .FirstOrDefaultAsync(p => p.Id == id);
            
            if (post == null)
            {
                throw new Exception("Post not found");
            }
            
            // Check permissions
            if (post.AuthorId != userId && userRole != "Admin" && userRole != "Moderator")
            {
                throw new UnauthorizedAccessException("You don't have permission to edit this post");
            }
            
            post.Title = postDto.Title;
            post.Content = postDto.Content;
            post.UpdatedAt = DateTime.UtcNow;
            
            // Update tags
            if (postDto.TagIds != null)
            {
                // Remove existing tags
                var existingTags = _context.PostTags.Where(pt => pt.PostId == id);
                _context.PostTags.RemoveRange(existingTags);
                
                // Add new tags
                foreach (var tagId in postDto.TagIds)
                {
                    var tag = await _context.Tags.FindAsync(tagId);
                    if (tag != null)
                    {
                        _context.PostTags.Add(new PostTag
                        {
                            PostId = id,
                            TagId = tagId
                        });
                    }
                }
            }
            
            await _context.SaveChangesAsync();
            return await GetPostByIdAsync(id);
        }
        
        public async Task<bool> DeletePostAsync(int id, int userId, string userRole)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return false;
            }
            
            // Check permissions
            if (post.AuthorId != userId && userRole != "Admin")
            {
                throw new UnauthorizedAccessException("You don't have permission to delete this post");
            }
            
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return true;
        }
        
        public async Task<IEnumerable<Post>> SearchPostsAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await GetAllPostsAsync();
            }
            
            return await _context.Posts
                .Where(p => p.Title.Contains(searchTerm) || p.Content.Contains(searchTerm))
                .Include(p => p.Author)
                .Include(p => p.PostTags)
                .ThenInclude(pt => pt.Tag)
                .Include(p => p.Comments)
                .ThenInclude(c => c.Author)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }
        
        public async Task<IEnumerable<Post>> GetPostsByTagAsync(int tagId)
        {
            return await _context.Posts
                .Where(p => p.PostTags.Any(pt => pt.TagId == tagId))
                .Include(p => p.Author)
                .Include(p => p.PostTags)
                .ThenInclude(pt => pt.Tag)
                .Include(p => p.Comments)
                .ThenInclude(c => c.Author)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }
    }
}