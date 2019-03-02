using BLLViews.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BLLViews.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult JobsList()
        {
            using (ApplicationDbContext ctx = new ApplicationDbContext())
            {

                var model = new JobsListModel()
                {

                    jobs = ctx.Jobs.Include("UserOwner").Include("Category").Include("City").ToList(),
                    Categories = ctx.Categories.ToList(),
                    Cities = ctx.Cities.ToList(),
                    Users = ctx.Users.ToList()
                };
                return View(model);
            }    
        }
    }
}