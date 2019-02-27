using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BLLViews.Models
{
    public class Job
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public decimal Salary { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public ApplicationUser User { get; set; }

        
    }
}