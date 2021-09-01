using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using staff_leave_app.Authentication;

namespace staff_leave_app.Model
{
    public class Staff
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "FirstName is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "LastName is required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "PhoneNumber is required")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "PhoneNumber is required")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Dob is required")]
        public DateTime Dob { get; set; }

        [Required(ErrorMessage = "CasualLeave is required")]
        public int CasualLeave { get; set; }

        [Required(ErrorMessage = "AnnualLeave is required")]
        public int AnnualLeave { get; set; }
        //navigation properties
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public List<Leave> Leave { get; set; }
    }
}
