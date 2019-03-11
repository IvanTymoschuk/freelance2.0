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
        public ActionResult AllLinks()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult NotFound()
        {
            return View();
        }
        [Authorize]
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


        [Authorize]
        public ActionResult JobView()
        {
            if (User.IsInRole("Banned"))
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



        const int pageSize = 9;

        public ActionResult JobsList(string search,int? page=0)
        {
            if (User.IsInRole("Banned"))
            {
                return Redirect("~/Account/banned");
            }

            var model = new JobsListModel();
            ApplicationDbContext ctx = new ApplicationDbContext();

            int pageIndex = pageSize * page.Value;
            model.jobs = ctx.Jobs.Include("UserOwner")
                .Include("Category")
                .Include("City")
                .Where(x=>string.IsNullOrEmpty(search) ? true : x.Name.Contains(search))
                .OrderByDescending(x => x.Date)
                .Skip(pageIndex)
                .Take(pageSize).ToList();
            model.Categories = ctx.Categories.ToList();
            model.Cities = ctx.Cities.ToList();
            model.Users = ctx.Users.ToList();
             
            if (Request.IsAjaxRequest())
            {
                return PartialView("_Items", model);
            }
          
            return View(model);
        }
    }
}
