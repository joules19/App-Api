using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;

namespace Models
{
    public class JwtConfig
    {
        public string Key { get; set; }
        public string Issuer { get; set; }

        public string ExpiryTimeFrame { get; set; }
    }
}

