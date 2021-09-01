using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using staff_leave_app.Authentication;

namespace staff_leave_app.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
  
    public class UserProfileController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> UserManager;
        private readonly ApplicationDbContext dbcontext;
        public UserProfileController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            this.UserManager = userManager;
            this.dbcontext = context;
        }
        [HttpGet]
        [Authorize]
        //Get : /api/UserProfile
        public async Task<Object> GetUserProfile()
        {
           // string userName = User.Claims.First(c => c.Type == "Jti").Value;
            string userId = User.FindFirst(ClaimTypes.Name)?.Value;
            var user = await UserManager.FindByIdAsync(userId);
            return (new
            {
                user.Email,
                user.UserName,
            });
        }
        [HttpGet]
        [Route("getall-users")]
        //Get : /api/UserProfile
        public async Task<Object> GetAllUsers()
        {
            var user = await dbcontext.Users.ToListAsync();
            return Ok(new { total = user.Count(), data = user});
        }
    }
}
