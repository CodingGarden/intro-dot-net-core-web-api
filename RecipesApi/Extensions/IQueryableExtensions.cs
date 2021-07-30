using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using RecipeApp.Models;

namespace RecipeApp.Extensions
{
    public static class IQueryableExtensions {
          public static Task<T> FirstOrDefaultWithIdAsync<T>(this IQueryable<T> source, int id) where T: IBaseModel {
            return source.FirstOrDefaultAsync((recipe) => recipe.Id == id);
          }
    }
}