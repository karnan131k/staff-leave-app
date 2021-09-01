using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using staff_leave_app.Authentication;

namespace staff_leave_app.Controllers
{
   
    [Route("api/[controller]")]
    [ApiController]
 
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> UserManager;
        public readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration _configuration;
        public AuthenticationController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            this.UserManager = userManager;
            this.roleManager = roleManager;
            _configuration = configuration;
        }
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var userExist = await UserManager.FindByNameAsync(model.UserName);
            if (userExist !=null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response{ Status = "Error", Message="User already exists" });
            }
            ApplicationUser user = new ApplicationUser()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.UserName
            };
            var result = await UserManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User Creation Failed" });
            }
            if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            }
            if (!await roleManager.RoleExistsAsync(UserRoles.User))
            {
                await roleManager.CreateAsync(new IdentityRole(UserRoles.User));
            }
            if (await roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await UserManager.AddToRoleAsync(user, UserRoles.User);
                
            }
            return Ok( new Response { Status = "Success", Message = "User Created Successfully" });
        }
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await UserManager.FindByNameAsync(model.UserName);
            if (user != null && await UserManager.CheckPasswordAsync(user,model.Password))
            {
                var userRoles = await UserManager.GetRolesAsync(user);
                var authClass = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,user.Id),
                    new Claim(JwtRegisteredClaimNames.Jti,user.Id),
                   // new Claim(JwtRegisteredClaimNames.GivenName,userRoles[0]),
                };
                foreach (var userRole in userRoles)
                {
                    authClass.Add(new Claim(JwtRegisteredClaimNames.GivenName, userRole));
                }
                var authSigninKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
                var token = new JwtSecurityToken(
                    issuer:_configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires:DateTime.Now.AddHours(3),
                    claims:authClass,
                    signingCredentials:new SigningCredentials(authSigninKey, SecurityAlgorithms.HmacSha256)
                    );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo,
                    user = user.UserName,
          

                });
            }
            return Unauthorized();
        }

        [HttpPost]
        [Route("RegisterAdmin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterModel model)
        {
            var userExist = await UserManager.FindByNameAsync(model.UserName);
            if (userExist != null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Admin User already exists" });
            }
            ApplicationUser user = new ApplicationUser()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.UserName
            };
            var result = await UserManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Admin User Creation Failed" });
            }
            if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            }
            if (!await roleManager.RoleExistsAsync(UserRoles.User))
            {
                await roleManager.CreateAsync(new IdentityRole(UserRoles.User));
            }
            if (await roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await UserManager.AddToRoleAsync(user, UserRoles.Admin);

            }
            return Ok(new Response { Status = "Success", Message = "Admin User Created Successfully" });
        }
    }
}
