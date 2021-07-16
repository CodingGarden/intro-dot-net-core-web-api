using System;
using System.ComponentModel.DataAnnotations;

namespace RecipeApp.DTO
{
    public class RecipeDTO {
      [Required, MaxLength(100)]
      public string Title { get; set; }
      [Required, MaxLength(1000)]
      public string Content { get; set; }
      [MaxLength(2048)]
      public string ImageUrl { get; set; }
    }
}