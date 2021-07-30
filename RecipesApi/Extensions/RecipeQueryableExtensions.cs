using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using RecipeApp.Models;

namespace RecipeApp.Extensions
{
    public static class RecipeQueryableExtensions {
          public static IIncludableQueryable<Recipe, User> IncludeCreator(this IQueryable<Recipe> source) {
            return source.Include((recipe) => recipe.Creator);
          }
    }
}