using Domain;
using Domain.Login;
using Domain.SignUp;
using Domin;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Infrastructure.Implementations
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JWT _jwt;
        public AuthenticationService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IOptions<JWT> jwt)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwt = jwt.Value;
        }

        private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
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
                new Claim("uid", user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(_jwt.DurationInDays),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }

        public async Task<string> AddToRoleAsync(AddRole role)
        {
            var user = await _userManager.FindByIdAsync(role.UserId);

            if (user is null || !await _roleManager.RoleExistsAsync(role.Role))
                return "Invalid user ID or Role";

            var currentRoles = await _userManager.GetRolesAsync(user);

            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
                return "Failed to remove existing roles";

            var addResult = await _userManager.AddToRoleAsync(user, role.Role);
            return addResult.Succeeded ? string.Empty : "Failed to assign new role";
        }

        public async Task<Authentication> RegisterAsync(Registering registering)
        {
            if (await _userManager.FindByEmailAsync(registering.Email) is not null)
                return new Authentication { Message = "Email is already registered!" };

            if (await _userManager.FindByNameAsync(registering.Username) is not null)
                return new Authentication { Message = "Username is already registered!" };

            var user = new ApplicationUser
            {
                UserName = registering.Username,
                Email = registering.Email,
                FullName = registering.FullName,
            };

            var result = await _userManager.CreateAsync(user, registering.Password);

            if (!result.Succeeded)
            {
                var errors = string.Empty;

                foreach (var error in result.Errors)
                    errors += $"{error.Description},";

                return new Authentication { Message = errors };
            }

            await _userManager.AddToRoleAsync(user, "User");

            var jwtSecurityToken = await CreateJwtToken(user);

            return new Authentication
            {
                Id = user.Id,
                Name = user.FullName,
                Email = user.Email,
                ExpiresOn = jwtSecurityToken.ValidTo,
                IsAuthenticated = true,
                Roles = new List<string> { "User" },
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                UserName = user.UserName
            };
        }

        public async Task<Authentication> SignUpAsync(SignIn signingUp)
        {
            var authModel = new Authentication();

            var user = await _userManager.FindByEmailAsync(signingUp.Email);

            if (user is null || !await _userManager.CheckPasswordAsync(user, signingUp.Password))
            {
                authModel.Message = "Email or Password is incorrect!";
                return authModel;
            }

            var jwtSecurityToken = await CreateJwtToken(user);
            var rolesList = await _userManager.GetRolesAsync(user);
            authModel.Id = user.Id;
            authModel.Name = user.FullName;
            authModel.IsAuthenticated = true;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authModel.Email = user.Email;
            authModel.UserName = user.UserName;
            authModel.ExpiresOn = jwtSecurityToken.ValidTo;
            authModel.Roles = rolesList.ToList();

            return authModel;
        }


        //public async Task<Authentication> UpdateAsync(UserUpdate update)
        //{
        //    var user = await _userManager.FindByIdAsync(update.Id);
        //    if (user == null)
        //        throw new ArgumentNullException(nameof(user));

        //    // Update profile fields
        //    user.UserName = update.Username;
        //    user.Email = update.Email;
        //    user.FirstName = update.FirstName;
        //    user.LastName = update.LastName;
        //    user.PhoneNumber = update.Contact;

        //    // Update password if it's different (optional logic)
        //    if (!string.IsNullOrWhiteSpace(update.Password))
        //    {
        //        var isOldPasswordCorrect = await _userManager.CheckPasswordAsync(user, update.OldPassword);

        //        if (isOldPasswordCorrect) 
        //        {
        //            // Only reset password if it's different from the current one
        //            var isSamePassword = await _userManager.CheckPasswordAsync(user, update.Password);

        //            if (!isSamePassword)
        //            {
        //                //var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        //                var result = await _userManager.ChangePasswordAsync(user, update.OldPassword, update.Password);

        //                if (!result.Succeeded)
        //                {
        //                    return new Authentication
        //                    {
        //                        IsAuthenticated = false,
        //                        Message = string.Join(", ", result.Errors.Select(e => e.Description))
        //                    };
        //                }
        //            }
        //        }
        //    }


        //    // Update the user
        //    var updateResult = await _userManager.UpdateAsync(user);
        //    if (!updateResult.Succeeded)
        //    {
        //        return new Authentication
        //        {
        //            Message = string.Join(", ", updateResult.Errors.Select(e => e.Description)),
        //            IsAuthenticated = false
        //        };
        //    }

        //    return new Authentication
        //    {
        //        IsAuthenticated = true,
        //        Email = user.Email,
        //        UserName = user.UserName,
        //        Roles = (await _userManager.GetRolesAsync(user)).ToList(),
        //        Message = "User updated successfully"
        //    };

        //}

        public async Task<Authentication> UpdateAsync(UserUpdate update)
        {
            var user = await _userManager.FindByIdAsync(update.Id);
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            // Update profile fields
            user.UserName = update.Username;
            user.Email = update.Email;
            user.FullName = update.FullName;
            user.PhoneNumber = update.Contact;

            // Handle password change
            if (!string.IsNullOrWhiteSpace(update.Password))
            {
                var isOldPasswordCorrect = await _userManager.CheckPasswordAsync(user, update.OldPassword);
                if (!isOldPasswordCorrect)
                {
                    return new Authentication
                    {
                        IsAuthenticated = false,
                        Message = "Old password is incorrect."
                    };
                }

                var result = await _userManager.ChangePasswordAsync(user, update.OldPassword, update.Password);
                if (!result.Succeeded)
                {
                    return new Authentication
                    {
                        IsAuthenticated = false,
                        Message = string.Join(", ", result.Errors.Select(e => e.Description))
                    };
                }
            }

            // Update user info
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                return new Authentication
                {
                    IsAuthenticated = false,
                    Message = string.Join(", ", updateResult.Errors.Select(e => e.Description))
                };
            }

            return new Authentication
            {
                IsAuthenticated = true,
                Email = user.Email,
                UserName = user.UserName,
                Roles = (await _userManager.GetRolesAsync(user)).ToList(),
                Message = "User updated successfully"
            };
        }

    }
}
