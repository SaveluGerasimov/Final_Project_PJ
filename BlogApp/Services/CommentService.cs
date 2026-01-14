using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BlogApp.Data;
using BlogApp.Models;

namespace BlogApp.Services
{
    public interface ICommentService
    {
        Task<Comment> CreateCommentAsync(string content, int postId, int authorId);
        Task<IEnumerable<Comment>> GetAllCommentsAsync();
        Task<Comment> GetCommentByIdAsync(int id);
        Task<Comment> UpdateCommentAsync(int id, string content, int userId, string userRole);
        Task<bool> DeleteCommentAsync(int id, int userId, string userRole);
        Task<IEnumerable<Comment>> GetCommentsByPostAsync(int postId);
    }
    
    public class CommentService : ICommentService
    {
        private readonly ApplicationDbContext _context;
        
        public CommentService(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<Comment> CreateCommentAsync(string content, int postId, int authorId)
        {
            var comment = new Comment
            {
                Content = content,
                PostId = postId,
                AuthorId = authorId,
                CreatedAt = DateTime.UtcNow
            };
            
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            
            return await GetCommentByIdAsync(comment.Id);
        }
        
        public async Task<IEnumerable<Comment>> GetAllCommentsAsync()
        {
            return await _context.Comments
                .Include(c => c.Author)
                .Include(c => c.Post)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }
        
        public async Task<Comment> GetCommentByIdAsync(int id)
        {
            return await _context.Comments
                .Include(c => c.Author)
                .Include(c => c.Post)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
        
        public async Task<Comment> UpdateCommentAsync(int id, string content, int userId, string userRole)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                throw new Exception("Comment not found");
            }
            
            // Check permissions
            if (comment.AuthorId != userId && userRole != "Admin" && userRole != "Moderator")
            {
                throw new UnauthorizedAccessException("You don't have permission to edit this comment");
            }
            
            comment.Content = content;
            comment.UpdatedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            return await GetCommentByIdAsync(id);
        }
        
        public async Task<bool> DeleteCommentAsync(int id, int userId, string userRole)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return false;
            }
            
            // Check permissions
            if (comment.AuthorId != userId && userRole != "Admin")
            {
                throw new UnauthorizedAccessException("You don't have permission to delete this comment");
            }
            
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return true;
        }
        
        public async Task<IEnumerable<Comment>> GetCommentsByPostAsync(int postId)
        {
            return await _context.Comments
                .Where(c => c.PostId == postId)
                .Include(c => c.Author)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }
    }
}