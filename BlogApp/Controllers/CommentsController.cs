using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BlogApp.Services;

namespace BlogApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;
        
        public CommentsController(ICommentService commentService)
        {
            _commentService = commentService;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAllComments()
        {
            var comments = await _commentService.GetAllCommentsAsync();
            return Ok(comments);
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCommentById(int id)
        {
            var comment = await _commentService.GetCommentByIdAsync(id);
            
            if (comment == null)
            {
                return NotFound(new { message = "Comment not found" });
            }
            
            return Ok(comment);
        }
        
        [HttpGet("post/{postId}")]
        public async Task<IActionResult> GetCommentsByPost(int postId)
        {
            var comments = await _commentService.GetCommentsByPostAsync(postId);
            return Ok(comments);
        }
        
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateComment([FromBody] CreateCommentDto commentDto)
        {
            try
            {
                var authorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var comment = await _commentService.CreateCommentAsync(
                    commentDto.Content, 
                    commentDto.PostId, 
                    authorId);
                return CreatedAtAction(nameof(GetCommentById), new { id = comment.Id }, comment);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateComment(int id, [FromBody] UpdateCommentDto commentDto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                var comment = await _commentService.UpdateCommentAsync(
                    id, 
                    commentDto.Content, 
                    userId, 
                    userRole);
                return Ok(comment);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteComment(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                var result = await _commentService.DeleteCommentAsync(id, userId, userRole);
                
                if (!result)
                {
                    return NotFound(new { message = "Comment not found" });
                }
                
                return Ok(new { message = "Comment deleted successfully" });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid();
            }
        }
    }
    
    public class CreateCommentDto
    {
        public string Content { get; set; }
        public int PostId { get; set; }
    }
    
    public class UpdateCommentDto
    {
        public string Content { get; set; }
    }
}