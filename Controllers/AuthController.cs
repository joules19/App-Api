
using Microsoft.AspNetCore.Mvc;
using Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using api_app.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace csharp_crud_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    private IConfiguration _config;

    public AuthController(IConfiguration config, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _config = config;
        _roleManager = roleManager;
        _userManager = userManager;
    }

    [HttpPost]
    [Route("register")]
    public async Task<ActionResult> register([FromBody] RegisterModel model)
    {
        var userExist = await _userManager.FindByNameAsync(model.Username);
        if (userExist != null) return StatusCode(StatusCodes.Status500InternalServerError, new Response()
        {
            status = "Error",
            message = "Username already taken"
        });

        ApplicationUser user = new()
        {
            Email = model.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = model.Username
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded) return StatusCode(StatusCodes.Status500InternalServerError, new Response()
        {
            status = "Error",
            message = "Check data submitted"
        });

        if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
        {
            await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
        }
        if (!await _roleManager.RoleExistsAsync(UserRoles.User))
        {
            await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));
        }

        if (await _roleManager.RoleExistsAsync(UserRoles.User))
        {
            await _userManager.AddToRoleAsync(user, UserRoles.User);
        }

        return Ok(new Response()
        {
            status = "Success",
            message = "User created successfully"
        });
    }

    [HttpPost]
    [Route("login")]
    public async Task<ActionResult> login([FromBody] LoginModel model)
    {
        var user = await _userManager.FindByNameAsync(model.Username);
        if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var authSignInKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));

            var token = new JwtSecurityToken(
                 issuer: _config["Jwt:Issuer"],
                 audience: _config["Jwt:Issuer"],
                 expires: DateTime.Now.AddHours(3),
                 claims: authClaims,
                 signingCredentials: new SigningCredentials(authSignInKey, SecurityAlgorithms.HmacSha256)
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo,
                user = user.UserName
            });
        }

        return Unauthorized();
    }

    [HttpPost]
    [Route("register-admin")]
    public async Task<ActionResult> registerAdmin([FromBody] RegisterModel model)
    {
        var userExist = await _userManager.FindByNameAsync(model.Username);
        if (userExist != null) return StatusCode(StatusCodes.Status500InternalServerError, new Response()
        {
            status = "Error",
            message = "Username already taken"
        });

        ApplicationUser user = new()
        {
            Email = model.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = model.Username
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded) return StatusCode(StatusCodes.Status500InternalServerError, new Response()
        {
            status = "Error",
            message = "Check data submitted"
        });

        if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
        {
            await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
        }
        if (!await _roleManager.RoleExistsAsync(UserRoles.User))
        {
            await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));
        }

        if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
        {
            await _userManager.AddToRoleAsync(user, UserRoles.Admin);
        }

        return Ok(new Response()
        {
            status = "Success",
            message = "User created successfully"
        });
    }

    [Authorize(Roles = UserRoles.Admin)]
    [HttpGet]
    [Route("logins")]
    public async Task<ActionResult> logins()
    {
        return Ok("Success");
    }
}