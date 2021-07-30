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
using RecipeApp.Extensions;
using Microsoft.AspNetCore.Http;
using RecipeApp.Attributes;

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
        .IncludeCreator()
        .ToListAsync();
      return Ok(recipes);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ResponseMessageDTO), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Recipe), StatusCodes.Status200OK)]
    [Produces("application/json")]
    public async Task<ActionResult<Recipe>> Get(int id) {
      var recipe = await Context
        .Recipes
        .IncludeCreator()
        .FirstOrDefaultWithIdAsync(id);

      if (recipe == null) {
        return NotFound(ResponseMessages.NotFound);
      }
      return Ok(recipe);
    }

    [HttpPost]
    [Authorize]
    [WithUser]
    public async Task<ActionResult<Recipe>> Post(RecipeDTO recipeRequest) {
      // TODO: this is ugly... get it working with an extension method OR a user param
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
    [ProducesResponseType(typeof(ResponseMessageDTO), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ResponseMessageDTO), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Recipe), StatusCodes.Status200OK)]
    [Produces("application/json")]
    [WithUser]
    public async Task<ActionResult<Recipe>> Update(int id, [FromBody] RecipeDTO recipeRequest) {
      HttpContext.Items.TryGetValue("user", out var userObj);
      User user = (User)userObj;
      var recipe = await Context
        .Recipes
        .IncludeCreator()
        .FirstOrDefaultWithIdAsync(id);
      if (recipe == null) {
        return NotFound(ResponseMessages.NotFound);
      }
      if (recipe.CreatorId != user.Id) {
        return Unauthorized(ResponseMessages.UnAuthorized);
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
    [ProducesResponseType(typeof(ResponseMessageDTO), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ResponseMessageDTO), StatusCodes.Status401Unauthorized)]
    [Produces("application/json")]
    [WithUser]
    public async Task<ActionResult> Delete(int id) {
      var recipe = await Context
        .Recipes
        .FirstOrDefaultWithIdAsync(id);
      if (recipe == null) {
        return NotFound(ResponseMessages.NotFound);
      }
      HttpContext.Items.TryGetValue("user", out var userObj);
      User user = (User)userObj;
      if (recipe.CreatorId != user.Id) {
        return Unauthorized(ResponseMessages.UnAuthorized);
      }
      Context.Recipes.Remove(recipe);
      await Context.SaveChangesAsync();
      return Ok();
    }
  }
}