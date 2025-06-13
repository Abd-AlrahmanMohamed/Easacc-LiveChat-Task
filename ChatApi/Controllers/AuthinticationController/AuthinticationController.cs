using Domain;
using Domain.Login;
using Domain.Routing.BaseRouter;
using Domain.SignUp;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Task.Api.Controllers.AuthinticationController
{
    //[Route("api/[controller]")]
    [ApiController]
    public class AuthinticationController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuthenticationService _authenticationService;

        public AuthinticationController(UserManager<ApplicationUser> userManager, IAuthenticationService authenticationService)
        {
            _userManager = userManager;
            _authenticationService = authenticationService;
        }

        //[Authorize(Roles = "Admin")]
        [HttpGet(Router.UserRouter.GetAllUsers)]
        public async Task<IActionResult> GetAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            if (users == null || users.Count == 0)
            {
                return NotFound("No users found.");
            }

            return Ok(users);
        }

        //[Authorize(Roles = "User")]
        [HttpGet(Router.UserRouter.GetUserById)]
        public async Task<ActionResult<ApplicationUser>> GetUserById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound("User dose not exist");

            var result = new ApplicationUser
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                UserName = user.UserName
            };

            return Ok(result);
        }


        [HttpPost(Router.UserRouter.SignIn)]
        public async Task<IActionResult> RegisterAsync(Registering registering)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);



            var result = await _authenticationService.RegisterAsync(registering);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost(Router.UserRouter.AddToRole)]
        public async Task<IActionResult> AddToRoleAsync(AddRole addRole)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authenticationService.AddToRoleAsync(addRole);

            if (!string.IsNullOrEmpty(result))
                return BadRequest(result);

            return Ok(addRole);
        }

        [HttpPost(Router.UserRouter.Login)]
        public async Task<IActionResult> SignUpAsync(SignIn signUp)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authenticationService.SignUpAsync(signUp);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpPut(Router.UserRouter.UpdateUser)]
        public async Task<IActionResult> UpdateUser( UserUpdate userUpdate)
        {
            var result = await _authenticationService.UpdateAsync(userUpdate);

            if (result.IsAuthenticated)
            {
                return Ok(new { message = result.Message, user = result });
            }
            return BadRequest(new { message = result.Message });
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete(Router.UserRouter.DeleteUser)]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound("User not found.");

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("User deleted successfully.");
        }


    }
}
