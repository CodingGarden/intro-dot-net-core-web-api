using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RecipeApp.Data;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeApp.Middlewares
{
    public class SetUserMiddleware
    {
        private readonly RequestDelegate _next;

        public SetUserMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, RecipesContext dbContext)
        {
            // TODO: figure out a way that doesn't do this on every request...
            if (context.User != null) {
                var idClaim = context
                    .User
                    .Claims
                    .FirstOrDefault((claim) => claim.Type == "id");
                if (idClaim != null) {
                    // WARNING: this DB lookup happens on every request...
                    var user = await dbContext
                        .Users
                        .FirstOrDefaultAsync((user) => user.Id == int.Parse(idClaim.Value));
                    context.Items.Add("user", user);
                }
            }
            await _next(context);
        }
    }
}