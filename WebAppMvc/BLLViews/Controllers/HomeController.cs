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
        public ActionResult AboutCarrers()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        public ActionResult AboutOurTeam()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        public ActionResult AboutPress()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        public ActionResult AboutContactUs()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult NotFound()
        {
            return View();
        }
        public ActionResult CreateJobs()
        {
            if(User.IsInRole("Banned"))
            {
                return Redirect("~/Account/banned");
            }
            using (ApplicationDbContext ctx = new ApplicationDbContext())
            {

                var model = new JobsListModel()
                {
                   
                    jobs = ctx.Jobs.Include("Category").Include("City").ToList(),
                    Categories = ctx.Categories.ToList(),
                    Cities = ctx.Cities.ToList()
                };
                return View(model);
            }
        }
        public ActionResult JobsList()
        {
            if (User.IsInRole("Banned"))
            {
                return Redirect("~/Account/banned");
            }
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