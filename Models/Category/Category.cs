using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api_app.Models
{
    public class Category
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string CategoryName { get; set; }

    }
}