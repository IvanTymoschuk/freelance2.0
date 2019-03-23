using System;
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
    public class JobMSG
    {
        public JobMSG()
        {
            msgs = new List<JobMSGS>();
        }
        public ICollection<JobMSGS> msgs { get; set; }
        public JobMSGS NewMSG { get; set; }
    }
    public class ResumeListModel
    {
        public ResumeListModel()
        {
            list = new List<Resume>();
        }
        public ICollection<Resume> list { get; set; }
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
        public string date { get; set; }
        public string Description { get; set; }
        public int CityID { get; set; }
        public int CategoryID { get; set; }

        public ICollection<SelectListItem> Cities { get; set; }

        public ICollection<SelectListItem> Categories { get; set; }
    }


}
