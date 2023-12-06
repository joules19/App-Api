using System.ComponentModel.DataAnnotations;

namespace api_app.Models.DTOs.AuthDto
{

    public class AuthenticatedUserDto
    {
        public AuthenticatedUserDto()
        {
            Roles = new List<string>();
        }
        public string Id { get; set; }
        public string Email { get; set; }
        public string UserId { get; internal set; }
        public string StoreId { get; internal set; }

        public bool IsInternalStaff { get; set; }
        public List<string> Roles { get; set; }

    }

    public class AuthDto
    {
        [Required]
        public string StoreId { get; set; }
        [Required]
        public string LoginCredential { get; set; }
    }
}
