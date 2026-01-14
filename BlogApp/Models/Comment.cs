using System;
using System.ComponentModel.DataAnnotations;

namespace BlogApp.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string Content { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        
        public int PostId { get; set; }
        public int AuthorId { get; set; }
        
        // Navigation properties
        public Post Post { get; set; }
        public User Author { get; set; }
    }
}