using api_app.Models;
using api_app.Models.DTOs.UserDto;
using api_app.Models.User;

namespace api_app.Data
{
    public class UserMappings
    {
        public static UserDto Map(ApplicationUser src, UserDto destination)
        {
            destination.FirstName = src.FirstName;
            destination.LastName = src.LastName;
            destination.UserId = src.Id;
            destination.Email = src.Email;
            destination.StreetAddress = src.StreetAddress;
            destination.City = src.City;
            destination.State = src.State;
            destination.PostalCode = src.PostalCode;

            return destination;
        }

        public static ApplicationUser Map(UserDto src, ApplicationUser destination)
        {
            destination.FirstName = src.FirstName;
            destination.LastName = src.LastName;
            destination.Email = destination.Email;
            destination.PhoneNumber = src.PhoneNumber;
            destination.StreetAddress = src.StreetAddress;
            destination.City = src.City;
            destination.State = src.State;
            destination.PostalCode = src.PostalCode;

            return destination;
        }
    }
}
