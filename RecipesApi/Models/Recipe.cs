
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipeApp.Models
{    
  public class Recipe: BaseModel {
    [Required, MaxLength(100)]
    public string Title { get; set; }
    [Required, MaxLength(1000)]
    public string Content { get; set; }
    [MaxLength(2048)]
    public string ImageUrl { get; set; }
    [Required]
    public int CreatorId { get; set; }
    [ForeignKey("CreatorId")]
    public User Creator { get; set; }
  }
}
