
using System;
using System.ComponentModel.DataAnnotations;

namespace RecipeApp.Models
{    
  public interface IBaseModel {
    int Id { get; set; }
    DateTime CreatedAt { get; set; }
    DateTime UpdatedAt { get; set; }
  }
}
