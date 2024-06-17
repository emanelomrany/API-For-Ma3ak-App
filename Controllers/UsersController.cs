using Humanizer;
using Ma3ak.Models;
using Ma3ak.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace Ma3ak.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AuthService _authServices;
        private SignInManager<User> _signInManager;
        private SignInManager<User> SignInManager => _signInManager ??= HttpContext.RequestServices.GetService<SignInManager<User>>();
        private readonly UserManager<User> _userManager;
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;
        private new List<string> _allowedExtentions = new List<string> { ".jpg", ".png" };
        private long _maxAllowedPosterSize = 1048576;
        public UsersController(IAuthService authService , UserManager<User> userManager , AuthService authServices)
        {
            _authService = authService;
            _userManager = userManager;
            _authService = authServices;
        }

        
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            // استخراج رأس الطلب للحصول على الـ Token
            string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(new { StatusCode = 400, Message = "Authentication token not provided" });
            }

            // استدعاء الميثود GetProfileAsync من AuthService وتمرير التوكن
            var profile = await _authService.GetProfileAsync(token);

            if (!profile.IsAuthenticated)
            {
                return BadRequest(new { StatusCode = 400, profile.Message });
            }

            return Ok(new { StatusCode = 200, Message = "User profile retrieved successfully", User = profile });
        }


        [HttpGet("allUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _authService.GetAllUsersAsync();
            if (users == null || users.Count == 0)
            {
                return NotFound(new { StatusCode = 404, Message = "No users found" });
            }

            return Ok(new { StatusCode = 200, Message = "Users retrieved successfully", UsersCount = users.Count, Users = users });
        }

        [HttpPost("register")]
        public async Task<IActionResult> registerAsync([FromForm] RegisterModel model)
        {


            if (!ModelState.IsValid)

                return BadRequest(new { StatusCode = 400, ModelState });

            var result = await _authService.RegisterAsync(model);
            if (!result.IsAuthenticated)

                return BadRequest(new { StatusCode = 400, result.Message });


            

            return Ok(new { StatusCode = 200, Message = $"( Registration successful ) The user has already been added", User = result });



        }


        [HttpPost("Login")]
        public async Task<IActionResult> LoginTokenAsync([FromForm] TokenRequestModel model)
        {


            if (!ModelState.IsValid)

                return BadRequest(new { StatusCode = 400, ModelState });

            var result = await _authService.LoginTokenAsync(model);
            if (!result.IsAuthenticated)

                return BadRequest(new { StatusCode = 400, result.Message });




            return Ok(new { StatusCode = 200, Message = $"( Login successful )", User = result });



        }
        //[HttpPost("update-profile-picture")]
        //[Authorize]
        //public async Task<IActionResult> UpdateProfilePicture([FromForm] IFormFile profilePicture)
        //{
        //    // استخراج اسم المستخدم من الـ Token المصادق عليه
        //    var userName = User.Identity.Name;
        //    if (userName == null)
        //    {
        //        return Unauthorized(new { message = "User not authenticated" });
        //    }

        //    var result = await _authService.UpdateProfilePictureAsync(userName, profilePicture);

        //    if (!result.IsAuthenticated)
        //    {
        //        return BadRequest(new { message = result.Message });
        //    }

        //    return Ok(result);
        //}

        [HttpPost("logout")]
        public async Task<IActionResult> LogoutTokenAsync([FromForm] LogoutModel model)
        {


            if (!ModelState.IsValid)

                return BadRequest(new { StatusCode = 400, ModelState });

            var result2 = await _authService.LogoutTokenAsync(model);
            if (!result2.IsAuthenticated && result2.isDeleted == true)
            {
               // result2.isDeleted = true;

                return BadRequest(new { StatusCode = 400, result2.Message });
            }

            else if (result2.IsAuthenticated && result2.isDeleted == false)

                return Ok(new { StatusCode = 200, result2.Message });

         return BadRequest(new { Message = $"( Logout  )", User = result2 });



        }

     


        [HttpPost("addrole")]
        public async Task<IActionResult> AddRoleAsync([FromForm] AddRoleModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.AddRoleAsync(model);

            if (!string.IsNullOrEmpty(result))
                return BadRequest(result);

            return Ok(model);
        }

        [HttpPut("UpdateProfile")]
        public async Task<IActionResult> UpdateUserProfileAsync([FromForm] string userName, [FromForm] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { StatusCode = 400, ModelState });

            var result = await _authService.UpdateUserProfileAsync(userName, model);
            if (!result.IsAuthenticated)
                return BadRequest(new { StatusCode = 400, result.Message });

            

            return Ok(new { StatusCode = 200, Message = "User profile updated successfully", User = result });
        }
        [HttpPut("UpdateUserName")]
        public async Task<IActionResult> UpdateUserNameAsync([FromForm] string currentUserName, [FromForm] string newUserName)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { StatusCode = 400, ModelState });

            var result = await _authService.UpdateUserNameAsync(currentUserName, newUserName);
            if (!result.IsAuthenticated)
                return BadRequest(new { StatusCode = 400, result.Message });

            return Ok(new { StatusCode = 200, Message = "Username updated successfully", User = result });
        }
        [HttpPut("ChangePassword")]
        public async Task<IActionResult> ChangePasswordAsync([FromForm] string username, [FromForm] string newPassword)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { StatusCode = 400, ModelState });

            var result = await _authService.ChangePasswordAsync(username, newPassword);
            if (!result)
                return BadRequest(new { StatusCode = 400, Message = "Failed to change password." });

            return Ok(new { StatusCode = 200, Message = "Password changed successfully." });
        }
       






        [HttpDelete("DeleteUser")]
        public async Task<IActionResult> DeleteUserAsync([FromForm] string userName)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { StatusCode = 400, ModelState });

            var result = await _authService.DeleteUserAsync(userName);
            if (!result.IsAuthenticated)
                return BadRequest(new { StatusCode = 400, result.Message });

            return Ok(new { StatusCode = 200, Message = "User deleted successfully", User = result });
        }



        [HttpDelete("DeleteField")]
        public async Task<IActionResult> DeleteFieldAsync([FromForm] string userName, [FromForm] string fieldName)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { StatusCode = 400, ModelState });

            var result = await _authService.DeleteFieldByUserNameAsync(userName, fieldName);
            if (!result.IsAuthenticated)
                return BadRequest(new { StatusCode = 400, result.Message });

            return Ok(new { StatusCode = 200, Message = $"Field '{fieldName}' deleted successfully for user '{userName}'", User = result });
        }




    }
}
