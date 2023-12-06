using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace api_app.Authentication
{
    public class Response
    {
        public string? status { get; set; } = string.Empty;
        public string? message { get; set; } = string.Empty;
    }
}