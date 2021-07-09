using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RecipeApp.Data;
using RecipeApp.Models;
using RecipeApp.DTO;

namespace RecipesApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        RecipesContext Context { get; set; }

        public UsersController(RecipesContext context) {
          Context = context;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<UserDTO>>> Get()
        {
          var users = await Context
            .Users
            .Select(user => new UserDTO() {
              Id = user.Id,
              Username = user.Username,
              CreatedAt = user.CreatedAt,
              UpdatedAt = user.UpdatedAt,
            })
            .ToListAsync();
          return Ok(users);
        }

        // TODO: use identity... stop doing things manually
        [HttpPost]
        public async Task<ActionResult<UserDTO>> Post(User user)
        {
          user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
          await Context.Users.AddAsync(user);
          await Context.SaveChangesAsync();
          return Ok(new UserDTO() {
            Id = user.Id,
            Username = user.Username,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
          });
        }
    }
}
