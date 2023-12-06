using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class TokenRequest
    {
        [Required]
        public string Token { get; set; }
        [Required]

        public string RefreshToken { get; set; }
    }
}