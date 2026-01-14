using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlogApp.Models
{
    public class Tag
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        
        // Navigation properties
        public ICollection<PostTag> PostTags { get; set; }
    }
}