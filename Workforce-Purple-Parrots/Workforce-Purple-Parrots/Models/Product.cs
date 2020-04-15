﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Workforce_Purple_Parrots.Models
{
    public class Product
    {
        public int Id { get; set; }
        public DateTime DateAdded { get; set; }
        public int ProductTypeId { get; set; }
        public int CustomerId { get; set; }
        public double Price { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
