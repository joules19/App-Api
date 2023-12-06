using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;

namespace Models
{
    public class AuthResult
    {
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }


    }
}

