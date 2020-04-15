using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Workforce_Purple_Parrots.Models
{
    public class UserPaymentType
    {
        public int Id { get; set; }
        public string AcctNumber { get; set; }
        public bool Active { get; set; }
        public int CustomerId { get; set; }
        public int PaymentTypeId { get; set; }
    }
}
