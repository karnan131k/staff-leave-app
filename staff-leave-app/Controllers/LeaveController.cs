using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using staff_leave_app.Authentication;
using staff_leave_app.Model;

namespace staff_leave_app.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveController : ControllerBase
    {

        private readonly ApplicationDbContext dbcontext;
        private readonly UserManager<ApplicationUser> UserManager;
        public readonly RoleManager<IdentityRole> roleManager;
        public LeaveController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.dbcontext = context;
            this.UserManager = userManager;
            this.roleManager = roleManager;
        }
        [HttpPost]
        [Route("Update/{LeaveId:int}")]
        public async Task<IActionResult> UpdateLeave([FromBody] Leave model,int? LeaveId)
        {
            if (model == null)
            {
                return BadRequest(new { message = "Leave data required" });
            }
            //var staff = await dbcontext.Staffs.Where(e => e.UserId == model.StaffId).FirstOrDefaultAsync();
            var leave = await dbcontext.Leaves.Where(e => e.Id == LeaveId).FirstOrDefaultAsync();
            leave.FromDate = model.FromDate;
            leave.ToDate = model.ToDate;
            leave.Comments = model.Comments;
            leave.ApprovedBy = model.ApprovedBy;
            leave.Status = model.Status;
            leave.LeaveType = model.LeaveType;
            leave.StaffId = model.StaffId;
            dbcontext.Leaves.Update(leave);
            await dbcontext.SaveChangesAsync();
            return Ok(new { message = "Leave updated successfully" });
        }
        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> CreateLeave([FromBody] ApplyLeave model)
        {

            if (model.LeaveType == "1")
            {
                if (model.FromDate<model.ToDate)
                {
                    int day = (int)(model.ToDate - model.FromDate).TotalDays;
                    var staff = await dbcontext.Staffs.Where(e => e.UserId == model.StaffId).FirstOrDefaultAsync();
                    Leave leave = new Leave
                    {
                        FromDate = model.FromDate,
                        ToDate = model.ToDate,
                        Comments=model.Comments,
                        ApprovedBy = model.ApprovedBy,
                        Status =0,
                        LeaveType = model.LeaveType,
                        StaffId =staff.Id
                    };

                    var count =staff.CasualLeave - day;
                    if (count <= 0) 
                    {

                        return BadRequest(new { message = "You have entered additional "+ count +" casual days" , days=day , todate= model.ToDate , fromdate= model.FromDate});
                    }else if(staff.CasualLeave==0)
                    {
                        return BadRequest(new { message = "You  leave days are finished " + count + " casual days", days = day, todate = model.ToDate, fromdate = model.FromDate });
                    }
                    else
                    {
                        staff.CasualLeave = count;
                        dbcontext.Staffs.Update(staff);
                        dbcontext.SaveChanges();
                        await dbcontext.Leaves.AddAsync(leave);
                        await dbcontext.SaveChangesAsync();
                        return Ok(new { message="created casual leave", remainleave= staff.CasualLeave });
                    }
                }
            }
            else if(model.LeaveType == "2")
            {
                if (model.FromDate < model.ToDate)
                {
                    int days = (int)(model.ToDate - model.FromDate).TotalDays;
                    var staff = await dbcontext.Staffs.Where(e => e.UserId == model.StaffId).FirstOrDefaultAsync();
                    Leave leave = new Leave
                    {
                        FromDate = model.FromDate,
                        ToDate = model.ToDate,
                        Comments = model.Comments,
                        ApprovedBy = model.ApprovedBy,
                        Status = 0,
                        LeaveType = model.LeaveType,
                        StaffId = staff.Id
                    };
                    var diff = staff.AnnualLeave - days;
                    if (diff < 0)
                    {

                        return BadRequest(new { message = "You have entered additional " + diff + " anual days" });
                    }
                    else if (staff.AnnualLeave == 0)
                    {
                        return BadRequest(new { message = "You  leave days are finished " });
                    }
                    else
                    {
                        staff.AnnualLeave = diff;
                        dbcontext.Staffs.Update(staff);
                        dbcontext.SaveChanges();
                        await dbcontext.Leaves.AddAsync(leave);
                        await dbcontext.SaveChangesAsync();
                        return Ok(new { message = "created annual leave" });
                    }

                }
            }
            return BadRequest(new { message = "You leave type shoud be cassual or annual"});
        }
        [HttpGet]
        [Route("Index")]
        public async Task<IActionResult> GetLeaveDetails()
        {

            var data = await dbcontext.Leaves.ToListAsync();

            return Ok(new { total = data.Count(), data = data });
        }
        [HttpGet]
        [Route("GetLeaveDetailsBaesdOnStaffId/{StaffId:int}")]
        public async Task<IActionResult> GetLeaveDetailsBaesdOnStaffId(int? StaffId)
        {
            if (StaffId is null)
            {
                return BadRequest(new { message = "staff id is required" });
            }
            var leaves = await dbcontext.Leaves.Where(e => e.StaffId == StaffId).ToListAsync();
            return Ok(new { total = leaves.Count(), data = leaves });
        }
    }
}