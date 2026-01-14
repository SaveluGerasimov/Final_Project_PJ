using System.ComponentModel.DataAnnotations;

namespace BlogApp.DTOs
{
    public class RegisterDto
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Username { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }
        
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}