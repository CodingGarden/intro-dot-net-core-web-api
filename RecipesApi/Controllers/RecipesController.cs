using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using RecipeApp.Data;
using RecipeApp.DTO;
using RecipeApp.Models;

namespace RecipesApi.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class RecipesController : ControllerBase
  {
    public RecipesContext Context { get; set; }

    public RecipesController(RecipesContext context) {
      Context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Recipe>>> Get() {
      var recipes = await Context
        .Recipes
        .Include((recipe) => recipe.Creator)
        .ToListAsync();
      return Ok(recipes);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Recipe>> Get(int id) {
      var recipe = await Context
        .Recipes
        .Include((recipe) => recipe.Creator)
        .FirstOrDefaultAsync((recipe) => recipe.Id == id);
      if (recipe == null) {
        return NotFound(new { message = "Not Found" });
      }
      return Ok(recipe);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Recipe>> Post(RecipeDTO recipeRequest) {
      HttpContext.Items.TryGetValue("user", out var userObj);
      User user = (User)userObj;
      var recipe = new Recipe() {
        Title = recipeRequest.Title,
        Content = recipeRequest.Content,
        ImageUrl = recipeRequest.ImageUrl,
        CreatorId = user.Id,
        Creator = user,
      };
      await Context.Recipes.AddAsync(recipe);
      await Context.SaveChangesAsync();
      return Ok(recipe);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<Recipe>> Update(int id, [FromBody] RecipeDTO recipeRequest) {
      HttpContext.Items.TryGetValue("user", out var userObj);
      User user = (User)userObj;
      var recipe = await Context
        .Recipes
        .Include((recipe) => recipe.Creator)
        .FirstOrDefaultAsync((recipe) => recipe.Id == id);
      if (recipe == null) {
        return NotFound(new { message = "Not Found" });
      }
      if (recipe.CreatorId != user.Id) {
        return Unauthorized(new { message = "Unauthorized" });
      }
      recipe.Title = recipeRequest.Title;
      recipe.Content = recipeRequest.Content;
      recipe.ImageUrl = recipeRequest.ImageUrl;
      recipe.UpdatedAt = DateTime.UtcNow;
      await Context.SaveChangesAsync();
      return Ok(recipe);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult> Delete(int id) {
      var recipe = await Context
        .Recipes
        .FirstOrDefaultAsync((recipe) => recipe.Id == id);
      if (recipe == null) {
        return NotFound(new { message = "Not Found" });
      }
      HttpContext.Items.TryGetValue("user", out var userObj);
      User user = (User)userObj;
      if (recipe.CreatorId != user.Id) {
        return Unauthorized(new { message = "Unauthorized" });
      }
      Context.Recipes.Remove(recipe);
      await Context.SaveChangesAsync();
      return Ok();
    }
  }
}