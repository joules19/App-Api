using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Models.DTOs
{
    public class UserLoginRequestDto
    {
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}