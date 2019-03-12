﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BLLViews.Models
{
    public class JobsListModel
    {
        public List<Job> jobs { get; set; }
        public List<Category> Categories { get; set; }
        public List<City> Cities { get; set; }
        public List<ApplicationUser> Users { get; set; }

    }
    public class CreateJobModel
    {
        public CreateJobModel()
        {
            Cities = new List<SelectListItem>();

            Categories = new List<SelectListItem>();
        }
        public string Name { get; set; }
        public decimal Salary { get; set; }
        public DateTime date { get; set; }
        public string Description { get; set; }
        public int CityID { get; set; }
        public int CategoryID { get; set; }

        public ICollection<SelectListItem> Cities { get; set; }

        public ICollection<SelectListItem> Categories { get; set; }
    }


}
