using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
//using TestApiJWT.Helpers;
//using TestApiJWT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.General;
using Ma3ak.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Ma3ak.Models;
using Humanizer;
using Azure.Core;
using Microsoft.AspNetCore.Identity;
using NuGet.Protocol;



namespace Ma3ak.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly JWT _jwt;
        public AuthService (UserManager<User> userManager, IOptions<JWT> jwt, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _jwt = jwt.Value;
            _roleManager = roleManager;
        }


        
        public async Task<AuthModel> GetProfileAsync(string token)
        {
            var authModel = new AuthModel();

            if (!string.IsNullOrEmpty(token))
            {
                var handler = new JwtSecurityTokenHandler();
                var tokenString = token.Replace("Bearer ", "");
                var tokenData = handler.ReadJwtToken(tokenString);

                var userIdClaim = tokenData.Claims.FirstOrDefault(claim => claim.Type == "UserId");
                if (userIdClaim != null)
                {
                    var userId = userIdClaim.Value;
                    var user = await _userManager.FindByIdAsync(userId);

                    if (user != null)
                    {
                        authModel.Id = user.Id;
                        authModel.UserName = user.UserName;
                        authModel.FirstName = user.FirstName;
                        authModel.LastName = user.LastName;
                        authModel.Email = user.Email;
                        authModel.Password = user.Password;
                        authModel.Gender = user.Gender;
                        authModel.NationalID = user.NationalID;
                        authModel.PhoneNumber = user.PhoneNumber;
                        authModel.isDeleted = user.isDeleted;
                        authModel.Roles = (await _userManager.GetRolesAsync(user)).ToList();
                        authModel.Poster = user.Poster;
                        authModel.Token = token;
                        authModel.IsAuthenticated = true;
                    }
                    else
                    {
                        authModel.Message = "User not found";
                    }
                }
                else
                {
                    authModel.Message = "UserId claim not found in the token";
                }
            }
            else
            {
                authModel.Message = "Authentication token not provided";
            }

            return authModel;
        }



        public async Task<List<AuthModel>> GetAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            var authModels = new List<AuthModel>();

            foreach (var user in users)
            {
                var jwtSecurityToken = await CreateJwtToken(user);

                var authModel = new AuthModel
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                    UserName = user.UserName,
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Password = user.Password,
                    Gender = user.Gender,
                    NationalID = user.NationalID,
                    PhoneNumber = user.PhoneNumber,
                    isDeleted = user.isDeleted,
                    IsAuthenticated = true,
                    Roles = new List<string> { "User" },
                    Poster = user.Poster,
                    ExpiresOn = jwtSecurityToken.ValidTo
                };

                authModels.Add(authModel);
            }

            return authModels;
        }



      

        public async Task<AuthModel> RegisterAsync([FromForm] RegisterModel model)
        {
            if (await _userManager.FindByEmailAsync(model.Email) is not null)
                return new AuthModel { Message = "Email is already registered" };

            if (await _userManager.FindByNameAsync(model.UserName) is not null)
                return new AuthModel { Message = "UserName is already registered" };

            var user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Password = model.Password,
                Gender = model.Gender,
                NationalID = model.NationalID,
                PhoneNumber = model.PhoneNumber,
                UserName = model.UserName,
                isDeleted = false
            };

            if (model.Poster != null && model.Poster.Length > 0)
            {
                var fileName = $"{Guid.NewGuid()}_{model.Poster.FileName}";
                var filePath = Path.Combine("wwwroot/images", fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.Poster.CopyToAsync(fileStream);
                }
                user.Poster = $"/images/{fileName}";
            }
            else
            {
                user.Poster = null;
            }

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(",", result.Errors.Select(e => e.Description));
                return new AuthModel { Message = errors };
            }

            await _userManager.AddToRoleAsync(user, "User");
            var jwtSecurityToken = await CreateJwtToken(user);
            return new AuthModel
            {
                Email = user.Email,
                Id = user.Id,
                ExpiresOn = jwtSecurityToken.ValidTo,
                IsAuthenticated = true,
                Roles = new List<string> { "User" },
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Gender = user.Gender,
                isDeleted = false,
                NationalID = user.NationalID,
                PhoneNumber = user.PhoneNumber,
                Password = user.Password,
                Poster = user.Poster
            };
        }


       


        public async Task<AuthModel> LoginTokenAsync(TokenRequestModel model)
        {
            var authModel = new AuthModel { };
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                authModel.Message = "UserName or Password is incorrect";
                return authModel;
            }

            if (user.isDeleted)
            {
                user.isDeleted = false;
                await _userManager.UpdateAsync(user);
            }

            var jwtSecurityToken = await CreateJwtToken(user);
            var rolesList = await _userManager.GetRolesAsync(user);
            authModel.IsAuthenticated = true;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authModel.Email = user.Email;
            authModel.Password = user.Password;
            authModel.UserName = user.UserName;
            authModel.ExpiresOn = jwtSecurityToken.ValidTo;
            authModel.Roles = rolesList.ToList();
            authModel.FirstName = user.FirstName;
            authModel.LastName = user.LastName;
            authModel.Gender = user.Gender;
            authModel.PhoneNumber = user.PhoneNumber;
            authModel.NationalID = user.NationalID;
            authModel.isDeleted = user.isDeleted;
            authModel.Poster = user.Poster;

            return authModel;
        }





        public async Task<AuthModel> LogoutTokenAsync(LogoutModel model)
        {
            var authModel = new AuthModel { };
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null || user.isDeleted == true)
            {
                authModel.Message = "User not found";
                authModel.IsAuthenticated = false;
                return authModel;
            }

            user.isDeleted = true;
            await _userManager.UpdateAsync(user);

            await _userManager.RemoveAuthenticationTokenAsync(user, "SystemName", "access_token");

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                authModel.Message = "Failed to update user data";
                authModel.IsAuthenticated = false;
                return authModel;
            }

            var rolesList = await _userManager.GetRolesAsync(user);

            authModel.Token = null;

            var updatedUser = await _userManager.FindByNameAsync(model.UserName);

            authModel.Message = "User logged out successfully";
            authModel.IsAuthenticated = true;
            authModel.UserName = updatedUser.UserName;
            authModel.FirstName = updatedUser.FirstName;
            authModel.LastName = updatedUser.LastName;
            authModel.Email = updatedUser.Email;
            authModel.Password = updatedUser.Password;
            authModel.Roles = rolesList.ToList();
            authModel.Gender = updatedUser.Gender;
            authModel.PhoneNumber = updatedUser.PhoneNumber;
            authModel.NationalID = updatedUser.NationalID;
            authModel.Poster = updatedUser.Poster;
            authModel.isDeleted = true;

            return authModel;
        }

        public async  Task<string> AddRoleAsync(AddRoleModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user is null || !await _roleManager.RoleExistsAsync(model.Role))
                return "Invalid user ID or Role";

            if (await _userManager.IsInRoleAsync(user, model.Role))
                return "User already assigned to this role";

            var result = await _userManager.AddToRoleAsync(user, model.Role);

            return result.Succeeded ? string.Empty : "Sonething went wrong";

        }
        private async Task<JwtSecurityToken> CreateJwtToken(User user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                //new Claim("uid", user.Id)
                new Claim("UserId", user.Id)
            }
           
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
          
                expires: DateTime.Now.AddDays(_jwt.DurationInDays),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }

       

        public async Task<AuthModel> UpdateUserProfileAsync(string userName, RegisterModel model)
        {
            var authModel = new AuthModel();
            var existingUser = await _userManager.FindByNameAsync(userName);
            if (existingUser == null || existingUser.isDeleted)
            {
                authModel.Message = "User not found";
                return authModel;
            }

            existingUser.FirstName = model.FirstName;
            existingUser.LastName = model.LastName;
            existingUser.UserName = model.UserName;
            existingUser.Email = model.Email;
            existingUser.Password = model.Password;
            existingUser.Gender = model.Gender;
            existingUser.NationalID = model.NationalID;
            existingUser.PhoneNumber = model.PhoneNumber;
            existingUser.isDeleted = model.isDeleted;

            if (model.Poster != null && model.Poster.Length > 0)
            {
                var fileName = $"{Guid.NewGuid()}_{model.Poster.FileName}";
                var filePath = Path.Combine("wwwroot/images", fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.Poster.CopyToAsync(fileStream);
                }
                existingUser.Poster = $"/images/{fileName}";
            }
            else
            {
                existingUser.Poster = null;
            }

            var result = await _userManager.UpdateAsync(existingUser);
            if (!result.Succeeded)
            {
                var errors = string.Join(",", result.Errors.Select(e => e.Description));
                return new AuthModel { Message = errors };
            }

            var jwtSecurityToken = await CreateJwtToken(existingUser);
            var rolesList = await _userManager.GetRolesAsync(existingUser);
            authModel.IsAuthenticated = true;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authModel.UserName = existingUser.UserName;
            authModel.FirstName = existingUser.FirstName;
            authModel.LastName = existingUser.LastName;
            authModel.Email = existingUser.Email;
            authModel.Password = existingUser.Password;
            authModel.Gender = existingUser.Gender;
            authModel.PhoneNumber = existingUser.PhoneNumber;
            authModel.NationalID = existingUser.NationalID;
            authModel.isDeleted = existingUser.isDeleted;
            authModel.Roles = rolesList.ToList();
            authModel.Poster = existingUser.Poster;

            return authModel;
        }



        public async Task<AuthModel> UpdateUserNameAsync(string currentUserName, string newUserName)
        {
            var authModel = new AuthModel();
            var existingUser = await _userManager.FindByNameAsync(currentUserName);
            if (existingUser == null || existingUser.isDeleted)
            {
                authModel.Message = "User not found";
                return authModel;
            }

            // Check if the new username is available
            var existingUserWithNewName = await _userManager.FindByNameAsync(newUserName);
            if (existingUserWithNewName != null)
            {
                authModel.Message = "Username already exists";
                return authModel;
            }

            // Update UserName
            existingUser.UserName = newUserName;

            // Save changes
            var result = await _userManager.UpdateAsync(existingUser);
            if (!result.Succeeded)
            {
                var errors = string.Empty;
                foreach (var error in result.Errors)
                {
                    errors += $"{error.Description},";
                }
                return new AuthModel { Message = errors };
            }

            // Prepare authentication model for response

            var jwtSecurityToken = await CreateJwtToken(existingUser);
            var rolesList = await _userManager.GetRolesAsync(existingUser);
            authModel.IsAuthenticated = true;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authModel.UserName = existingUser.UserName;
            authModel.FirstName = existingUser.FirstName;
            authModel.LastName = existingUser.LastName;
            authModel.Email = existingUser.Email;
            authModel.Password = existingUser.Password;
            authModel.Gender = existingUser.Gender;
            authModel.NationalID = existingUser.NationalID;
            authModel.PhoneNumber = existingUser.PhoneNumber;
            authModel.isDeleted = existingUser.isDeleted;
            authModel.IsAuthenticated = true;
            authModel.Poster = existingUser.Poster;
            authModel.ExpiresOn = jwtSecurityToken.ValidTo;
            authModel.Roles = rolesList.ToList();

            return authModel;
        }

      


        public async Task<bool> ChangePasswordAsync(string username, string newPassword)
        {
            // تجاوز العملية إذا كان اسم المستخدم أو كلمة المرور الجديدة فارغة
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(newPassword))
            {
                return false;
            }

            // العثور على المستخدم باستخدام اسم المستخدم
            var existingUser = await _userManager.FindByNameAsync(username);
            if (existingUser == null || existingUser.isDeleted)
            {
                // في حالة عدم العثور على المستخدم أو كان محذوفًا، ارجاع قيمة false
                return false;
            }

            // إنشاء رمز إعادة تعيين كلمة المرور
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(existingUser);

            // إعادة تعيين كلمة المرور باستخدام الرمز وكلمة المرور الجديدة
            var resetPasswordResult = await _userManager.ResetPasswordAsync(existingUser, resetToken, newPassword);

            // التحقق مما إذا نجحت عملية إعادة تعيين كلمة المرور أم لا
            return resetPasswordResult.Succeeded;
        }

      

        public async Task<AuthModel> DeleteUserAsync(string userName)
        {
            var authModel = new AuthModel();
            var existingUser = await _userManager.FindByNameAsync(userName);
            if (existingUser == null)
            {
                authModel.Message = "User not found";
                return authModel;
            }

            // Set isDeleted flag to true
            existingUser.isDeleted = true;

            // Save changes
            var result = await _userManager.UpdateAsync(existingUser);
            if (!result.Succeeded)
            {
                var errors = string.Empty;
                foreach (var error in result.Errors)
                {
                    errors += $"{error.Description},";
                }
                return new AuthModel { Message = errors };
            }

            // Prepare authentication model for response
            authModel.Message = "User deleted successfully";
            authModel.UserName = existingUser.UserName;
            authModel.FirstName = existingUser.FirstName;
            authModel.LastName = existingUser.LastName;
            authModel.Email = existingUser.Email;
            authModel.Password  = existingUser.Password;
            authModel.Gender = existingUser.Gender;
            authModel.NationalID = existingUser.NationalID;
            authModel.PhoneNumber = existingUser.PhoneNumber;
            authModel.isDeleted = true;
            authModel.IsAuthenticated = true;
            authModel.Poster = existingUser.Poster;

            return authModel;
        }
        public async Task<AuthModel> DeleteFieldByUserNameAsync(string userName, string fieldName)
        {
            var authModel = new AuthModel();
            var existingUser = await _userManager.FindByNameAsync(userName);
            if (existingUser == null)
            {
                authModel.Message = "User not found";
                authModel.IsAuthenticated = false;
                return authModel;
            }

            // التأكد من أن اسم الحقل غير فارغ
            if (string.IsNullOrEmpty(fieldName))
            {
                return new AuthModel { Message = "Field name cannot be empty", IsAuthenticated = false };
            }

            // حذف البيانات من الحقل المحدد
            switch (fieldName)
            {
                case "Poster":
                    existingUser.Poster = null; // تعيين البوستر إلى null
                    break;

                // إضافة المزيد من الحقول إذا كان هناك حاجة إليها
                default:
                    return new AuthModel { Message = "Invalid field name", IsAuthenticated = false };
            }

            // حفظ التغييرات
            var result = await _userManager.UpdateAsync(existingUser);
            if (!result.Succeeded)
            {
                var errors = string.Join(",", result.Errors.Select(e => e.Description));
                return new AuthModel { Message = errors, IsAuthenticated = false };
            }

            // تحضير نموذج المصادقة للاستجابة
            authModel.UserName = existingUser.UserName;
            authModel.FirstName = existingUser.FirstName;
            authModel.LastName = existingUser.LastName;
            authModel.Email = existingUser.Email;
            authModel.Password = existingUser.Password;
            authModel.Gender = existingUser.Gender;
            authModel.NationalID = existingUser.NationalID;
            authModel.PhoneNumber = existingUser.PhoneNumber;
            authModel.isDeleted = existingUser.isDeleted;
            authModel.IsAuthenticated = true;
            authModel.Poster = existingUser.Poster;
            authModel.Message = $"Field '{fieldName}' deleted successfully for user '{userName}'";

            return authModel;
        }




        //public async Task<AuthModel> UpdateProfilePictureAsync(string userName, IFormFile profilePicture)
        //{
        //    var authModel = new AuthModel();
        //    var existingUser = await _userManager.FindByNameAsync(userName);
        //    if (existingUser == null || existingUser.isDeleted)
        //    {
        //        authModel.Message = "User not found";
        //        return authModel;
        //    }

        //    if (profilePicture != null && profilePicture.Length > 0)
        //    {
        //        var fileName = $"{Guid.NewGuid()}_{profilePicture.FileName}";
        //        var filePath = Path.Combine("wwwroot/images", fileName);
        //        using (var fileStream = new FileStream(filePath, FileMode.Create))
        //        {
        //            await profilePicture.CopyToAsync(fileStream);
        //        }
        //        existingUser.Poster = $"/images/{fileName}";
        //    }
        //    else
        //    {
        //        existingUser.Poster = null;
        //    }

        //    var result = await _userManager.UpdateAsync(existingUser);
        //    if (!result.Succeeded)
        //    {
        //        var errors = string.Join(",", result.Errors.Select(e => e.Description));
        //        return new AuthModel { Message = errors };
        //    }

        //    authModel.IsAuthenticated = true;
        //    authModel.UserName = existingUser.UserName;
        //    authModel.FirstName = existingUser.FirstName;
        //    authModel.LastName = existingUser.LastName;
        //    authModel.Email = existingUser.Email;
        //    authModel.Password = existingUser.Password;
        //    authModel.Gender = existingUser.Gender;
        //    authModel.PhoneNumber = existingUser.PhoneNumber;
        //    authModel.NationalID = existingUser.NationalID;
        //    authModel.isDeleted = existingUser.isDeleted;
        //    authModel.Poster = existingUser.Poster;

        //    return authModel;
        //}






    }
}

