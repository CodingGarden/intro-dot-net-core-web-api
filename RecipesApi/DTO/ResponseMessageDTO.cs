using System;

namespace RecipeApp.DTO
{
  public record ResponseMessageDTO(string Message, int StatusCode, string StackTrace = "");

  public static class ResponseMessages {
    public static readonly ResponseMessageDTO NotFound = new ResponseMessageDTO("Not Found", 404);
    public static readonly ResponseMessageDTO UnAuthorized = new ResponseMessageDTO("UnAuthorized", 401);
  }
}