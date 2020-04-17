using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Workforce_Purple_Parrots.Models.ViewModels
{
    public class ComputerFormViewModel
    {
        public int Id { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime PurchaseDate { get; set; }

        [Display(Name = "Employee")]
        [Required]
        public int EmployeeId { get; set; }
        public List<SelectListItem> EmployeeOptions { get; set; }
    }
}
