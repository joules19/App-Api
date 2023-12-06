using api_app.Helpers;
using api_app.Models;
using api_app.Models.DTOs.ApiResponseDto;
using api_app.Models.DTOs.AuthDto;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

namespace api_app.Controllers
{
    public class BaseApiController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        //private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger _logger;
        public BaseApiController(IWebHostEnvironment env, ILogger logger)
        {
            _env = env;
            _logger = logger;
        }
        protected string GetActiveAccount(string accountId)
        {
            var authUser = GetAuthenticatedUserDetails();

            if (accountId == null && authUser.IsInternalStaff)
            {
                throw new BadRequestException("provide account to act on");
            }

            if (!authUser.IsInternalStaff)
            {
                accountId = authUser.Id;
            }

            return accountId;
        }

        protected AuthenticatedUserDto GetAuthenticatedUserDetails()

        {
            var userDetails = new AuthenticatedUserDto();

            var isUserBuyer = User.IsInRole(RolesEnum.Buyer.ToString());
            var isUserSeller = User.IsInRole(RolesEnum.Seller.ToString());
            var isUserAdmin = User.IsInRole(RolesEnum.Admin.ToString());
            var isUserSuperAdmin = User.IsInRole(RolesEnum.SuperAdmin.ToString());


            if (isUserBuyer)
            {
                userDetails.UserId = User.Claims.FirstOrDefault(x => x.Type == "Id")?.Value;
                userDetails.Email = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
                userDetails.Roles.Add(RolesEnum.Buyer.ToString());
            }

            if (isUserSeller)
            {
                userDetails.UserId = User.Claims.FirstOrDefault(x => x.Type == "Id")?.Value;
                userDetails.Email = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
                userDetails.Roles.Add(RolesEnum.Seller.ToString());
            }

            if (isUserAdmin)
            {
                userDetails.IsInternalStaff = true;
                userDetails.UserId = User.Claims.FirstOrDefault(x => x.Type == "Id")?.Value;
                userDetails.Email = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
                userDetails.Roles.Add(RolesEnum.Admin.ToString());
            }

            else
            {
                userDetails.IsInternalStaff = true;
                userDetails.UserId = User.Claims.FirstOrDefault(x => x.Type == "Id")?.Value;
                userDetails.Email = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
                userDetails.StoreId = User.Claims.FirstOrDefault(x => x.Type == "StoreId")?.Value;
                userDetails.Roles.Add(RolesEnum.SuperAdmin.ToString());
            }

            return userDetails;
        }


        protected IActionResult GetExceptionResponse(Exception ex, dynamic payload)
        {
            if (ex is BadRequestException)
            {
                var exception = (BadRequestException)ex;
                string objString = null;
                if (payload != null)
                {
                    objString = JsonConvert.SerializeObject(payload);
                    _logger.LogError(objString);
                }
                var errors = new List<string>() { ex.Message };
                return StatusCode(400, new ApiErrorResponseDto("An error occured", errors));
            }
            else if (ex is UnauthorizedRequestException)
            {
                var exception = (UnauthorizedRequestException)ex;
                string objString = null;
                if (payload != null)
                {
                    objString = JsonConvert.SerializeObject(payload);
                    _logger.LogError(objString);
                }
                return StatusCode(401, new ApiScalarResponseDto(ex.Message, null, ResponseStatus.Failure));
            }


            return StatusCode(500, ex.ToString());
        }
    }
}
