using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BlogApp.Services;
using BlogApp.DTOs;

namespace BlogApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostsController : ControllerBase
    {
        private readonly IPostService _postService;
        
        public PostsController(IPostService postService)
        {
            _postService = postService;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAllPosts()
        {
            var posts = await _postService.GetAllPostsAsync();
            return Ok(posts);
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPostById(int id)
        {
            var post = await _postService.GetPostByIdAsync(id);
            
            if (post == null)
            {
                return NotFound(new { message = "Post not found" });
            }
            
            return Ok(post);
        }
        
        [HttpGet("author/{authorId}")]
        public async Task<IActionResult> GetPostsByAuthor(int authorId)
        {
            var posts = await _postService.GetPostsByAuthorAsync(authorId);
            return Ok(posts);
        }
        
        [HttpGet("search")]
        public async Task<IActionResult> SearchPosts([FromQuery] string q)
        {
            var posts = await _postService.SearchPostsAsync(q);
            return Ok(posts);
        }
        
        [HttpGet("tag/{tagId}")]
        public async Task<IActionResult> GetPostsByTag(int tagId)
        {
            var posts = await _postService.GetPostsByTagAsync(tagId);
            return Ok(posts);
        }
        
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreatePost(CreatePostDto postDto)
        {
            try
            {
                var authorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var post = await _postService.CreatePostAsync(postDto, authorId);
                return CreatedAtAction(nameof(GetPostById), new { id = post.Id }, post);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdatePost(int id, UpdatePostDto postDto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                var post = await _postService.UpdatePostAsync(id, postDto, userId, userRole);
                return Ok(post);
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
        public async Task<IActionResult> DeletePost(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                var result = await _postService.DeletePostAsync(id, userId, userRole);
                
                if (!result)
                {
                    return NotFound(new { message = "Post not found" });
                }
                
                return Ok(new { message = "Post deleted successfully" });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid();
            }
        }
    }
}