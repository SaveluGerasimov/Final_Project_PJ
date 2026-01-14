using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BlogApp.Services;

namespace BlogApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TagsController : ControllerBase
    {
        private readonly ITagService _tagService;
        
        public TagsController(ITagService tagService)
        {
            _tagService = tagService;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAllTags()
        {
            var tags = await _tagService.GetAllTagsAsync();
            return Ok(tags);
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTagById(int id)
        {
            var tag = await _tagService.GetTagByIdAsync(id);
            
            if (tag == null)
            {
                return NotFound(new { message = "Tag not found" });
            }
            
            return Ok(tag);
        }
        
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateTag([FromBody] CreateTagDto tagDto)
        {
            try
            {
                var tag = await _tagService.CreateTagAsync(tagDto.Name);
                return CreatedAtAction(nameof(GetTagById), new { id = tag.Id }, tag);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateTag(int id, [FromBody] UpdateTagDto tagDto)
        {
            try
            {
                var tag = await _tagService.UpdateTagAsync(id, tagDto.Name);
                return Ok(tag);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTag(int id)
        {
            var result = await _tagService.DeleteTagAsync(id);
            
            if (!result)
            {
                return NotFound(new { message = "Tag not found" });
            }
            
            return Ok(new { message = "Tag deleted successfully" });
        }
    }
    
    public class CreateTagDto
    {
        public string Name { get; set; }
    }
    
    public class UpdateTagDto
    {
        public string Name { get; set; }
    }
}