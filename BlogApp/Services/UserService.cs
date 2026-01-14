using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BlogApp.Data;
using BlogApp.Models;
using BlogApp.DTOs;

namespace BlogApp.Services
{
    public interface IUserService
    {
        Task<User> RegisterAsync(RegisterDto registerDto);
        Task<User> AuthenticateAsync(LoginDto loginDto);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> GetUserByIdAsync(int id);
        Task<User> UpdateUserAsync(int id, User user);
        Task<bool> DeleteUserAsync(int id);
    }
    
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        
        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<User> RegisterAsync(RegisterDto registerDto)
        {
            // Check if user already exists
            if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
            {
                throw new Exception("User with this email already exists");
            }
            
            // Create new user
            var user = new User
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                CreatedAt = DateTime.UtcNow
            };
            
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            
            // Assign default role (User)
            var userRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "User");
            if (userRole != null)
            {
                _context.UserRoles.Add(new UserRole
                {
                    UserId = user.Id,
                    RoleId = userRole.Id
                });
                await _context.SaveChangesAsync();
            }
            
            return user;
        }
        
        public async Task<User> AuthenticateAsync(LoginDto loginDto)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Email == loginDto.Email);
            
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                return null;
            }
            
            return user;
        }
        
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .ToListAsync();
        }
        
        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == id);
        }
        
        public async Task<User> UpdateUserAsync(int id, User user)
        {
            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null)
            {
                throw new Exception("User not found");
            }
            
            existingUser.Username = user.Username;
            existingUser.Email = user.Email;
            
            await _context.SaveChangesAsync();
            return existingUser;
        }
        
        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return false;
            }
            
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}