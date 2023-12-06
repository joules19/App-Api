using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using api_app.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

public class Store
{
    public int Id { get; set; }
    public string StoreId { get; set; }
    [Required]
    public string StoreName { get; set; }
    [Required]
    public string Location { get; set; }
    [Required]
    public string ApplicationUserId { get; set; }
    [ForeignKey("ApplicationUserId")]
    [ValidateNever]
    public ApplicationUser ApplicationUser { get; set; }

    public string Description { get; set; }
}