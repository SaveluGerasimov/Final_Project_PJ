using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlogApp.Models
{
    public class Post
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; }
        
        [Required]
        public string Content { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        
        public int AuthorId { get; set; }
        
        // Navigation properties
        public User Author { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<PostTag> PostTags { get; set; }
    }
}