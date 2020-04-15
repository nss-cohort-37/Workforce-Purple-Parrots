using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Workforce_Purple_Parrots.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    
        public int DepartmentId { get; set; }
        public Department Department { get; set; }
        public bool IsSupervisor { get; set; }
        public string Email { get; set; }
        public int ComputerId { get; set; }
        public Computer? Computer { get; set; }
        public List<TrainingProgram>? TrainingProgram { get; set; }

    }
}
