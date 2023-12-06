using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using api_app.Authentication;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using api_app.Data;
using Models.DTOs;
using Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using api_app.Helpers;
using api_app.Models.DTOs.ApiResponseDto;
using api_app.Models;

namespace api_app.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : BaseApiController
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IEmailSender _emailSender;
    private readonly IMailService _mailService;

    //private readonly JwtConfig _jwtConfig;
    private readonly TokenValidationParameters _tokenValidationParameters;
    private readonly ApplicationDbContext _context;
    private IConfiguration _config;
    private readonly IWebHostEnvironment _hostEnvironment;


    public AuthController(IMailService _MailService, IEmailSender emailSender, IConfiguration config, ApplicationDbContext context,
    UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, TokenValidationParameters tokenValidationParameters, IWebHostEnvironment hostEnvironment,
            ILogger<UsersController> logger) : base(hostEnvironment, logger)
    {
        _config = config;
        _roleManager = roleManager;
        _userManager = userManager;
        _mailService = _MailService;
        _context = context;
        _emailSender = emailSender;
        _tokenValidationParameters = tokenValidationParameters;
        _hostEnvironment = hostEnvironment;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiSingleResponseDto), StatusCodes.Status200OK)]
    [Route("register")]
    public async Task<IActionResult> register([FromBody] UserRegistrationRequestDto requestDto)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var userExist = await _userManager.FindByEmailAsync(requestDto.Email);
                if (userExist != null)
                {
                    throw new BadRequestException("Email already exist");
                }
            }

            ApplicationUser user = new()
            {
                Email = requestDto.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = requestDto.Username
            };

            var result = await _userManager.CreateAsync(user, requestDto.Password);

            if (!result.Succeeded)
            {
                throw new BadRequestException("Server Error");
            }

            if (!await _roleManager.RoleExistsAsync(RolesEnum.Seller.ToString()))
            {
                await _roleManager.CreateAsync(new IdentityRole(RolesEnum.Seller.ToString()));
            }
            if (!await _roleManager.RoleExistsAsync(RolesEnum.Admin.ToString()))
            {
                await _roleManager.CreateAsync(new IdentityRole(RolesEnum.Admin.ToString()));
            }
            if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            }
            if (!await _roleManager.RoleExistsAsync(RolesEnum.SuperAdmin.ToString()))
            {
                await _roleManager.CreateAsync(new IdentityRole(RolesEnum.SuperAdmin.ToString()));
            }

            if (await _roleManager.RoleExistsAsync(RolesEnum.Buyer.ToString()))
            {
                await _userManager.AddToRoleAsync(user, RolesEnum.Buyer.ToString());
            }

            var responseModel = new ApiSingleResponseDto("Operation Success");
            return Ok(responseModel);

        }
        catch (System.Exception ex)
        {
            return GetExceptionResponse(ex, null);
        }
    }


    [HttpPost]
    [Route("login")]
    [ProducesResponseType(typeof(ApiClassResponseDto<AuthResult>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Login([FromBody] UserLoginRequestDto requestDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                throw new BadRequestException("Server Error");
            }

            var existing_user = await _userManager.FindByEmailAsync(requestDto.Email);

            if (existing_user == null)
            {
                throw new BadRequestException("invalid credentials");
            }

            var isPasswordCorrect = await _userManager.CheckPasswordAsync(existing_user, requestDto.Password);

            if (!isPasswordCorrect) throw new BadRequestException("invalid password provided");

            var result = await GenerateJwtToken(existing_user);
            var responseModel = new ApiClassResponseDto<AuthResult>("Operation Success", result);
            return Ok(responseModel);

        }
        catch (Exception ex)
        {
            return GetExceptionResponse(ex, null);
        }
    }


    [HttpPost]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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


        if (!await _roleManager.RoleExistsAsync(RolesEnum.Seller.ToString()))
        {
            await _roleManager.CreateAsync(new IdentityRole(RolesEnum.Seller.ToString()));
        }
        if (!await _roleManager.RoleExistsAsync(RolesEnum.Buyer.ToString()))
        {
            await _roleManager.CreateAsync(new IdentityRole(RolesEnum.Buyer.ToString()));

        }
        if (!await _roleManager.RoleExistsAsync(RolesEnum.Admin.ToString()))
        {
            await _roleManager.CreateAsync(new IdentityRole(RolesEnum.Admin.ToString()));
        }
        if (!await _roleManager.RoleExistsAsync(RolesEnum.SuperAdmin.ToString()))
        {
            await _roleManager.CreateAsync(new IdentityRole(RolesEnum.SuperAdmin.ToString()));
        }

        if (await _roleManager.RoleExistsAsync(RolesEnum.Buyer.ToString()))
        {
            await _userManager.AddToRoleAsync(user, RolesEnum.Buyer.ToString());
        }
        if (await _roleManager.RoleExistsAsync(RolesEnum.Admin.ToString()))
        {
            await _userManager.AddToRoleAsync(user, RolesEnum.Admin.ToString());
        }

        return Ok(new Response()
        {
            status = "Success",
            message = "User created successfully"
        });
    }


    [HttpPost]
    //[Authorize]
    [Route("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] TokenRequest tokenRequest)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                throw new BadRequestException("Invalid credentials");
            }

            var result = await VerifyAndGenerateToken(tokenRequest);
            if (result == null) throw new BadRequestException("invalid operation");

            return Ok(result);
        }

        catch (System.Exception ex)
        {
            return GetExceptionResponse(ex, null);
        }


    }

    #region AUTH HELPERS
    private async Task<AuthResult> VerifyAndGenerateToken(TokenRequest tokenRequest)
    {
        var jwtHandler = new JwtSecurityTokenHandler();

        _tokenValidationParameters.ValidateLifetime = false;
        var tokenInVerification = jwtHandler.ValidateToken(tokenRequest.Token, _tokenValidationParameters, out var validatedToken);
        if (validatedToken is JwtSecurityToken jwtSecurityToken)
        {
            var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);

            if (result == false)
            {
                return null;
            }
        }

        var utcExpiryDate = long.Parse(tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
        var expiryDate = UnixTimeStampToDateTime(utcExpiryDate);
        if (expiryDate > DateTime.Now)
        {
            throw new BadRequestException("Expired Time Stamp");
        }

        var storedToken = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == tokenRequest.RefreshToken);
        if (storedToken == null)
        {
            throw new BadRequestException("Invalid Tokens");
        }

        if (storedToken.IsUsed)
        {
            throw new BadRequestException("Invalid Tokens");
        }

        if (storedToken.IsRevoked)
        {
            throw new BadRequestException("Invalid Tokens");
        }

        var jti = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
        if (storedToken.JwtId != jti)
        {
            throw new BadRequestException("Invalid Tokens");

        }
        if (storedToken.ExpiryDate < DateTime.UtcNow)
        {
            throw new BadRequestException("Invalid Tokens");
        }

        storedToken.IsUsed = true;
        _context.RefreshTokens.Update(storedToken);
        await _context.SaveChangesAsync();

        var dbUser = await _userManager.FindByIdAsync(storedToken.UserId);
        var authToken = await GenerateJwtToken(dbUser);

        return authToken;
    }

    private DateTime UnixTimeStampToDateTime(long unixTimeStamp)
    {
        var dateTimeVal = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        dateTimeVal = dateTimeVal.AddSeconds(unixTimeStamp).ToUniversalTime();

        return dateTimeVal;
    }

    private async Task<AuthResult> GenerateJwtToken(ApplicationUser user)
    {
        var jwtTokenHandler = new JwtSecurityTokenHandler();
        //var key = Encoding.UTF8.GetBytes(_jwtConfig.Key);
        var authSignInKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        //Token Descriptor
        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(new[] {
             new Claim("Id", user.Id),
             new Claim(JwtRegisteredClaimNames.Sub, user.Email),
             new Claim(JwtRegisteredClaimNames.Email, user.Email),
             new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
             new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToUniversalTime().ToString()),
            }),

            Expires = DateTime.UtcNow.Add(TimeSpan.Parse(_config["Jwt:ExpiryTimeFrame"])),
            SigningCredentials = new SigningCredentials((authSignInKey), SecurityAlgorithms.HmacSha256)
        };

        if (await _userManager.IsInRoleAsync(user, RolesEnum.Seller.ToString()))
        {
            tokenDescriptor.Subject.AddClaim(new Claim("Role", RolesEnum.Seller.ToString()));
            tokenDescriptor.Subject.AddClaim(new Claim(ClaimTypes.Role, RolesEnum.Seller.ToString()));
        }
        if (await _userManager.IsInRoleAsync(user, RolesEnum.Buyer.ToString()))
        {
            tokenDescriptor.Subject.AddClaim(new Claim("Role", RolesEnum.Buyer.ToString()));
            tokenDescriptor.Subject.AddClaim(new Claim(ClaimTypes.Role, RolesEnum.Buyer.ToString()));
        }

        if (await _userManager.IsInRoleAsync(user, RolesEnum.Admin.ToString()))
        {
            tokenDescriptor.Subject.AddClaim(new Claim("Role", RolesEnum.Admin.ToString()));
            tokenDescriptor.Subject.AddClaim(new Claim(ClaimTypes.Role, RolesEnum.Admin.ToString()));
        }
        if (await _userManager.IsInRoleAsync(user, RolesEnum.SuperAdmin.ToString()))
        {
            tokenDescriptor.Subject.AddClaim(new Claim("Role", RolesEnum.SuperAdmin.ToString()));
            tokenDescriptor.Subject.AddClaim(new Claim(ClaimTypes.Role, RolesEnum.SuperAdmin.ToString()));
        }

        var token = jwtTokenHandler.CreateToken(tokenDescriptor);
        var jwtToken = jwtTokenHandler.WriteToken(token);

        var refreshToken = new RefreshToken()
        {
            JwtId = token.Id,
            Token = randomStringGeneration(23),
            AddedDate = DateTime.UtcNow,
            ExpiryDate = DateTime.UtcNow.AddMonths(6),
            IsRevoked = false,
            IsUsed = false,
            UserId = user.Id,
        };

        await _context.RefreshTokens.AddAsync(refreshToken);
        await _context.SaveChangesAsync();

        var authResult = new AuthResult()
        {
            RefreshToken = refreshToken.Token,
            Token = jwtToken,
        };

        return authResult;
    }

    private string randomStringGeneration(int length)
    {
        var random = new Random();
        var chars = "AECQEEGHJJKLUNQRQRSTUVWXNZ1234567890gcdefabiiklwngpacstwwwxz_";
        return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
    }

    #endregion
}

// StringBuilder template = new();
// template.AppendLine("Dear @Model.FirstName,");
// template.AppendLine("<p>Thanks for purchasing @Model.ProductName. We hope you enjoy it.</p›");
// template.AppendLine("The Timco Team");
// await _emailSender.SendEmailAsync("babafemiayodele1@gmail.com", "Hello", "Body");
