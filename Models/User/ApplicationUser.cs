using Microsoft.AspNetCore.Identity;

namespace api_app.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? StreetAddress { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }

        // public int? CompanyId { get; set; }
        // [ForeignKey("CompanyId")]
        // [ValidateNever]
        // public Company Company { get; set; }
    }
}