namespace BlogApp.Models
{
    public class UserRole
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        
        // Navigation properties
        public User User { get; set; }
        public Role Role { get; set; }
    }
}