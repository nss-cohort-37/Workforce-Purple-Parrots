using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Workforce_Purple_Parrots.Models.ViewModels
{
    public class EmployeeFormViewModel
    {
        public int EmployeeId { get; set; }

        [Display(Name = "First Name")]
        [Required]
        [MinLength(2)]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Last Name is Required")]
        [MinLength(3, ErrorMessage = "Last Name should be at least 3 characters")]
        public string LastName { get; set; }

        [Display(Name = "Department Id")]
        [Required]
       
        public int DepartmentId { get; set; }

        [Display(Name = "Computer Id")]
        [Required]

        public int ComputerId { get; set; }

        [Display(Name = "E-mail")]
        [Required]
        [MinLength(2)]
        public string Email { get; set; }

        [Display(Name = "Supervisor status")]
    
        public bool IsSupervisor { get; set; }

    }
}
