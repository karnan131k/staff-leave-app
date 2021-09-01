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
using staff_leave_app.Model;

namespace staff_leave_app.Controllers
{
   // [Authorize(Roles = UserRoles.User)]
   //[Authorize]
    //[EnableCors("AllowOrigin")]
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    //[Authorize(Roles = UserRoles.User)]
    public class StaffController : ControllerBase
    {
        private readonly ApplicationDbContext dbcontext;
        private readonly UserManager<ApplicationUser> UserManager;
        public readonly RoleManager<IdentityRole> roleManager;
        public StaffController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.dbcontext = context;
            this.UserManager = userManager;
            this.roleManager = roleManager;
        }
        [HttpPost]
        [Route("Create")]
        public async Task<ActionResult<Staff>> Create([FromBody] Staff model)
        {
            //return Ok(new { message = "Staff created successfully", data = model });
            if (model == null)
            {
                return BadRequest(new { message = " staff details is required" });
            }
            try
            {
                if (await roleManager.RoleExistsAsync(UserRoles.Admin))
                {
                    //var user = await UserManager.FindByIdAsync(model.UserId);
                    var staff =await dbcontext.Staffs.Where(e => e.UserId == model.UserId).FirstOrDefaultAsync();
                    if (staff is null)
                    {
                        await dbcontext.Staffs.AddAsync(model);
                    }
                    else
                    {
                        staff.FirstName = model.FirstName;
                        staff.LastName = model.LastName;
                        staff.PhoneNumber = model.PhoneNumber;
                        staff.Address = model.Address;
                        staff.Dob = model.Dob;
                        staff.CasualLeave = model.CasualLeave;
                        staff.AnnualLeave = model.AnnualLeave;
                        staff.UserId = model.UserId;

                        dbcontext.Staffs.Update(staff);
                    } 
                }
                else
                {
                    var staff = await dbcontext.Staffs.Where(e => e.UserId == model.UserId).FirstOrDefaultAsync();
                    if (staff is null)
                    {
                        await dbcontext.Staffs.AddAsync(model);
                    }
                    else
                    {
                        staff.FirstName = model.FirstName;
                        staff.LastName = model.LastName;
                        staff.PhoneNumber = model.PhoneNumber;
                        staff.Address = model.Address;
                        staff.Dob = model.Dob;
                        staff.CasualLeave = model.CasualLeave;
                        staff.AnnualLeave = model.AnnualLeave;
                        staff.UserId = model.UserId;

                        dbcontext.Staffs.Update(staff);
                    };

                }
                await dbcontext.SaveChangesAsync();
                return Ok(new { message = $"Staff details updated successfully", data = model });
                //string userId = User.FindFirst(ClaimTypes.Name)?.Value;
                //var user = await UserManager.FindByIdAsync(userId);
                // model.UserId = user.Id;
                //return Ok(new { message = "Staff created successfully", data = model });

                //await dbcontext.Staffs.AddAsync(model);
                //await dbcontext.SaveChangesAsync();
                //  return Ok(new { message = $"Staff created successfully" });
                //return Ok(new { message = $"Staff created successfully", data=model });
            }
            catch (Exception ex)
            {

                return Ok(new { message= $"{ex.Message.ToString()}" });
            }
        }
        [HttpGet]
        [Route("Index")]
        //Get : /api/UserProfile
        public async Task<ActionResult<Staff>> GetStaffDetails()
        {
            // string userName = User.Claims.First(c => c.Type == "Jti").Value;
            /*string userId = User.FindFirst(ClaimTypes.Name)?.Value;
            var user = await UserManager.FindByIdAsync(userId);
            if (user.Id== userId)
            {

            }*/
            var data = await dbcontext.Staffs.ToListAsync();
            
            return Ok(new { total = data.Count(), data=data });
        }
    }
}
