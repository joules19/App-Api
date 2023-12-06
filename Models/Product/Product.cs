using api_app.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api_app.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string ProductId { get; set; }
        [Required]
        public string ProductName { get; set; }
        public string Description { get; set; }
        public string Image01Url { get; set; }
        public string Image02Url { get; set; }
        [Required]
        [Range(1, 10000)]
        public double SlashedPrice { get; set; }

        [Required]
        [Range(1, 10000)]
        public double Price { get; set; }

        [Required]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        [ValidateNever]
        public Category Category { get; set; }

        [Required]
        public int StoreId { get; set; }
        [ForeignKey("StoreId")]
        [ValidateNever]
        public Store Store { get; set; }
    }
}