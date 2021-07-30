using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using RecipeApp.Data;
using RecipeApp.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace RecipeApp.Attributes
{
  public class WithUserAttribute : ActionFilterAttribute
  {
      public override void OnActionExecuting(ActionExecutingContext executingContext)
      {
        var context = executingContext.HttpContext;
        if (context.User != null) {
            var idClaim = context
                .User
                .Claims
                .FirstOrDefault((claim) => claim.Type == "id");
            if (idClaim != null) {
              var _dbContext = context.RequestServices.GetService<RecipesContext>();
              var user = _dbContext
                .Users
                .FirstOrDefault(user => user.Id == int.Parse(idClaim.Value));
              context.Items.Add("user", user);
            }
        }
        base.OnActionExecuting(executingContext);
      }
  }   
}