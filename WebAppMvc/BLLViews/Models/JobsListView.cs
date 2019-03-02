using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace BLLViews.Models
{
    public class JobsListModel
    {
        public List<Job> jobs { get; set; }
        public List<Category> Categories { get; set; }
        public List<City> Cities { get; set; }
        public List<ApplicationUser> Users { get; set; }

    }

}
