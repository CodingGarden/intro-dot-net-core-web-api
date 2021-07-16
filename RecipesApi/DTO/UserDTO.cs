using System;

namespace RecipeApp.DTO
{
    public record LoginUserDTO(string Username, string Password);

    public class UserDTO {
      public int Id { get; set; }
      public string Username { get; set; }
      public DateTime CreatedAt { get; set; }
      public DateTime UpdatedAt { get; set; }
    }
}