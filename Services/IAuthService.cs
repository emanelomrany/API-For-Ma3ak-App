using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace Ma3ak.Services
{
    public interface IAuthService
    {
       // Task<AuthModel> GetProfileAsync(HttpContext httpContext);
        Task<AuthModel> GetProfileAsync(string token);
        Task<List<AuthModel>> GetAllUsersAsync();
        Task<AuthModel> RegisterAsync([FromForm] RegisterModel model);
        Task<AuthModel> LoginTokenAsync(TokenRequestModel model);
      
        Task<string> AddRoleAsync(AddRoleModel model);
        Task<AuthModel> LogoutTokenAsync(LogoutModel model);
        Task<AuthModel> UpdateUserProfileAsync(string userName, RegisterModel model);
        Task<AuthModel> UpdateUserNameAsync(string currentUserName, string newUserName);
      
        Task<AuthModel> DeleteUserAsync(string userName);
        Task<AuthModel> DeleteFieldByUserNameAsync(string userName, string fieldName);
        //Task<bool> DeleteFieldByUserNameAsync(string userName, string fieldName);
        Task<bool> ChangePasswordAsync(string username, string newPassword);

      ///  Task<AuthModel> UpdateProfilePictureAsync(string userName, IFormFile profilePicture);

    }
}
