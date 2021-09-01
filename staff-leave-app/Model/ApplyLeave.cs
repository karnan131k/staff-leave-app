using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace staff_leave_app.Model
{
    public class ApplyLeave
    {
        [Required(ErrorMessage = "FromDate is required")]
        public DateTime FromDate { get; set; }

        [Required(ErrorMessage = "ToDate is required")]
        public DateTime ToDate { get; set; }

        [Required(ErrorMessage = "Comments is required")]
        public string Comments { get; set; }

        [Required(ErrorMessage = "LeaveType is required")]
        public string LeaveType { get; set; }

        [Required(ErrorMessage = "ApprovedBy is required")]
        public string ApprovedBy { get; set; }
        public string StaffId { get; set; }
    }
}
