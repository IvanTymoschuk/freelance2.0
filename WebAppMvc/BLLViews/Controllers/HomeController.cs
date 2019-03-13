using BLLViews.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BLLViews.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        public ActionResult Index()
        {
            return View();
        }
        [Authorize]
        public ActionResult GetMSG(int id)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                JobMSG model = new JobMSG();

                var list = db.Jobs.FirstOrDefault(x => x.ID == id).JobMSGS.ToList();
                model.msgs = list;
                model.NewMSG = new JobMSGS();
                model.NewMSG.job = new Job();
                model.NewMSG.job.ID = id;
                return PartialView("_Chat",model);
            }
             
        }
        [Authorize]
        public ActionResult AddMsgToChat(JobMSG model)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {

                db.JobMSGs.Add(new JobMSGS { job = db.Jobs.SingleOrDefault(x => x.ID == model.NewMSG.job.ID) });
                db.SaveChanges();
                return new EmptyResult();

            }
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
        public ActionResult TopSkill()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult NotFound()
        {
            return View();
        }
        [Authorize]
        [HttpPost]
        public ActionResult CreateJobs(CreateJobModel model)
        {
            using (ApplicationDbContext ctx = new ApplicationDbContext())
            {
                var uid = User.Identity.GetUserId();
                ctx.Jobs.Add(new Job
                {
                    Description = model.Description,
                    @Category = ctx.Categories.FirstOrDefault(x => x.ID == model.CategoryID),
                    Name = model.Name,
                    Salary = model.Salary,
                    @City = ctx.Cities.FirstOrDefault(x => x.Id == model.CityID),
                    Date = DateTime.Parse(model.date),
                    UserOwner = ctx.Users.FirstOrDefault(x => x.Id == uid)
                });
                ctx.SaveChanges();
            }
            return Redirect("/");
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

                var model = new CreateJobModel();
                foreach (var el in ctx.Cities)
                    model.Cities.Add(new SelectListItem() { Text = el.Name, Value = el.Id.ToString() });
                foreach (var el in ctx.Categories)
                    model.Categories.Add(new SelectListItem() { Text = el.Name, Value = el.ID.ToString() });


                return View(model);
            }
        }


        [Authorize]
        public ActionResult JobView(int id)
        {
            if (User.IsInRole("Banned"))
            {
                return Redirect("~/Account/banned");
            }


            using (ApplicationDbContext ctx = new ApplicationDbContext())
            {
                
                var model = new JobView()
                {

                    job = ctx.Jobs.Include("UserOwner").Include("Category").Include("City").SingleOrDefault(x => x.ID == id),
                    Categories = ctx.Categories.ToList(),
                    Cities = ctx.Cities.ToList(),
                    Users=ctx.Users.ToList()
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

        public ActionResult SubscribeManager(string id,int job_id)
        {

            using (ApplicationDbContext ctx = new ApplicationDbContext())
            {


                var lox = ctx.Users.Single(x => x.UserName == id);
                if (!ctx.Jobs.Single(x => x.ID == job_id).subscribers.Contains(lox))
                {
                    ctx.Jobs.Single(x => x.ID == job_id).subscribers.Add(lox);

                    return Json(new {state=true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    ctx.Jobs.Single(x => x.ID == job_id).subscribers.Remove(lox);
                    return Json(new { state = false }, JsonRequestBehavior.AllowGet);
                }

                
            }
        }
    }
}
