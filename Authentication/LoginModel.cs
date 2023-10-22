using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace api_app.Authentication
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}