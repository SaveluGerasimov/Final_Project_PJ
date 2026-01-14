using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlogApp.DTOs
{
    public class PostDto
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; }
        
        [Required]
        public string Content { get; set; }
        
        public List<int> TagIds { get; set; }
    }
    
    public class CreatePostDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; }
        
        [Required]
        public string Content { get; set; }
        
        public List<int> TagIds { get; set; }
    }
    
    public class UpdatePostDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; }
        
        [Required]
        public string Content { get; set; }
        
        public List<int> TagIds { get; set; }
    }
}