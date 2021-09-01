using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace staff_leave_app.Model
{
    public class Leave
    {
        public int Id { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Comments { get; set; }
        public string ApprovedBy { get; set; }
        public int Status { get; set; }
        public string LeaveType { get; set; }

        //navigation properties
        public int StaffId { get; set; }
        //public Staff Staff { get; set; }
    }
}
