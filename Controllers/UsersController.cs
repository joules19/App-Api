using Microsoft.AspNetCore.Mvc;
using Models;
using api_app.Data.Repository;
using Microsoft.AspNetCore.Identity;
using api_app.Data;
using Microsoft.AspNetCore.Authorization;
using api_app.Models.DTOs.ApiResponseDto;
using api_app.Models.DTOs.UserDto;
using api_app.Models;
using api_app.Models.User;

namespace api_app.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : BaseApiController
{
    private UnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IWebHostEnvironment _hostEnvironment;

    public UsersController(UnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IWebHostEnvironment hostEnvironment,
            ILogger<UsersController> logger) : base(hostEnvironment, logger)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _hostEnvironment = hostEnvironment;
    }


    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(ApiSingleResponseDto), StatusCodes.Status200OK)]
    [Route("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] AccountPasswordUpdateDto requestDto)
    {
        try
        {
            var account = GetAuthenticatedUserDetails();
            var user = await _userManager.FindByIdAsync(account.UserId.ToString());
            var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, requestDto.OldPassword);

            if (!isPasswordCorrect)
            {
                throw new BadRequestException("invalid password provided");
            }

            var result = await _userManager.ChangePasswordAsync(user, requestDto.OldPassword, requestDto.NewPassword);
            if (!result.Succeeded)
            {
                throw new Exception("Server Error");
            }
            var responseModel = new ApiSingleResponseDto("Operation Success");
            return Ok(responseModel);
        }
        catch (Exception ex)
        {
            return GetExceptionResponse(ex, null);
        }
    }


    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(ApiClassResponseDto<ApplicationUser>), StatusCodes.Status200OK)]
    [Route("user")]
    public async Task<IActionResult> GetUser()
    {
        try
        {
            var account = GetAuthenticatedUserDetails();
            var user = await _userManager.FindByIdAsync(account.UserId.ToString());
            var responseModel = new ApiClassResponseDto<ApplicationUser>("Operation Success", user);
            return Ok(responseModel);
        }
        catch (Exception ex)
        {
            return GetExceptionResponse(ex, null);
        }

    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(ApiSingleResponseDto), StatusCodes.Status200OK)]
    [Route("complete-registration")]
    public async Task<IActionResult> CompleteRegistration([FromBody] UserDto requestDto)
    {
        try
        {
            var account = GetAuthenticatedUserDetails();
            var user = await _userManager.FindByIdAsync(account.UserId.ToString());

            await _userManager.UpdateAsync(UserMappings.Map(requestDto, user));

            _unitOfWork.Save();


            var responseModel = new ApiSingleResponseDto("Operation Success");
            return Ok(responseModel);
        }
        catch (Exception ex)
        {
            return GetExceptionResponse(ex, null);
        }
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(ApiSingleResponseDto), StatusCodes.Status200OK)]
    [Route("update-user")]
    public async Task<IActionResult> UpdateUser([FromBody] UserDto requestDto)
    {
        try
        {
            var account = GetAuthenticatedUserDetails();
            var user = await _userManager.FindByIdAsync(account.UserId.ToString());

            await _userManager.UpdateAsync(UserMappings.Map(requestDto, user));
            _unitOfWork.Save();


            var responseModel = new ApiSingleResponseDto("Operation Success");
            return Ok(responseModel);
        }
        catch (Exception ex)
        {
            return GetExceptionResponse(ex, null);
        }
    }

    // POST api/users
    // [HttpPost]
    // public async Task<ActionResult<User>> PostUser(User user)
    // {
    //     // _context.Users.Add(user);
    //     // await _context.SaveChangesAsync();

    //     return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
    // }

    // PUT api/users/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutUser(int id, User user)
    {
        if (id != user.Id)
        {
            return BadRequest();
        }

        // _context.Entry(user).State = EntityState.Modified;
        // await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE api/users/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        //var user = await _context.Users.FindAsync(id);

        // if (user == null)
        // {
        //     return NotFound();
        // }

        // _context.Users.Remove(user);
        // await _context.SaveChangesAsync();

        return NoContent();
    }

}