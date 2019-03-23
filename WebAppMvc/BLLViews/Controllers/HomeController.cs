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
        public ActionResult RemoveJob(int id)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                if (User.IsInRole("Admin") || db.Jobs.FirstOrDefault(y => y.ID == id).UserOwner.Id==User.Identity.GetUserId())
                {

                    db.JobMSGs.RemoveRange(db.JobMSGs.Where(x => x.job == db.Jobs.FirstOrDefault(y => y.ID == id)));
                    db.Jobs.FirstOrDefault(y => y.ID == id).subscribers = null;
                    db.Jobs.FirstOrDefault(y => y.ID == id).Resumes = null;
                    db.Jobs.Remove(db.Jobs.FirstOrDefault(x => x.ID == id));
                    db.SaveChanges();

                }
                else
                {
                    return RedirectToAction("NotFound", "Home");
                }
            }
            return RedirectToAction("JobsList","Home");
        }
        [Authorize]
        public ActionResult GetMSG(int id)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                JobMSG model = new JobMSG();

                var list = db.JobMSGs.Include("job").Include("Sender").Where(x => x.job.ID == id).ToList();
                foreach (var el in list)
                    model.msgs.Add(el);

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
                var uid = User.Identity.GetUserId();
                db.Jobs.FirstOrDefault(x=>x.ID==model.NewMSG.job.ID).JobMSGS.Add(new JobMSGS { job = db.Jobs.SingleOrDefault(x => x.ID == model.NewMSG.job.ID), Sender = db.Users.FirstOrDefault(x => x.Id == uid), Text = model.NewMSG.Text });
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

                    job = ctx.Jobs.Include("UserOwner").Include("Resumes").Include("Category").Include("City").SingleOrDefault(x => x.ID == id),
                    Categories = ctx.Categories.ToList(),
                    Cities = ctx.Cities.ToList(),
                    Users=ctx.Users.ToList()
                };
                return View(model);
            }
        }



        const int pageSize = 9;

        public ActionResult JobsList(string search,int? page=0,string filter=null)
        {
            if (User.IsInRole("Banned"))
            {
                return Redirect("~/Account/banned");
            }

            var model = new JobsListModel();
            ApplicationDbContext ctx = new ApplicationDbContext();
            if (filter == null)
            {
                int pageIndex = pageSize * page.Value;
                model.jobs = ctx.Jobs.Include("UserOwner")
                    .Include("Category")
                    .Include("City")
                    .Where(x => string.IsNullOrEmpty(search) ? true : x.Name.Contains(search))
                    .OrderByDescending(x => x.Date)
                    .Skip(pageIndex)
                    .Take(pageSize).ToList();
                model.Categories = ctx.Categories.ToList();
                model.Cities = ctx.Cities.ToList();
                model.Users = ctx.Users.ToList();
            }
            else
            if (filter.Contains("city="))
            {
                var str = filter.Remove(0, 5);

                model.jobs = ctx.Jobs.Include("UserOwner")
                    .Include("Category")
                    .Include("City")
                    .Where(x => x.City == ctx.Cities.FirstOrDefault(y => y.Name == str))
                    .OrderByDescending(x => x.Date).ToList();

                model.Categories = ctx.Categories.ToList();
                model.Cities = ctx.Cities.ToList();
                model.Users = ctx.Users.ToList();
            }
            else
            if (filter.Contains("category="))
            {
                var str = filter.Remove(0, 9);

                model.jobs = ctx.Jobs.Include("UserOwner")
                    .Include("Category")
                    .Include("City")
                    .Where(x => x.Category == ctx.Categories.FirstOrDefault(y => y.Name == str))
                    .OrderByDescending(x => x.Date).ToList();

                model.Categories = ctx.Categories.ToList();
                model.Cities = ctx.Cities.ToList();
                model.Users = ctx.Users.ToList();
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView("_Items", model);
            }
          
            return View(model);
        }
        public ActionResult AcceptResume(int id)
        {
            

            using (ApplicationDbContext ctx = new ApplicationDbContext())
            {
                int jid = ctx.Resumes.Include("job").FirstOrDefault(x => x.Id == id).job.ID;
                if (ctx.Jobs.Include("UserOwner").FirstOrDefault(x => x.ID == jid).UserOwner.Id != User.Identity.GetUserId() || User.IsInRole("Admin")==false)
                    return RedirectToAction("notfound");
                    ctx.Resumes.FirstOrDefault(x => x.Id == id).Status = "ACCEPT";
                ctx.SaveChanges();
                return PartialView("ResumeList", GetResumeList(jid));
            }

         
        }
        public ActionResult DeinedResume(int id)
        {


            using (ApplicationDbContext ctx = new ApplicationDbContext())
            {

                int jid = ctx.Resumes.Include("job").FirstOrDefault(x => x.Id == id).job.ID;
                if (ctx.Jobs.Include("UserOwner").FirstOrDefault(x => x.ID == jid).UserOwner.Id != User.Identity.GetUserId() || User.IsInRole("Admin")==false)
                    return RedirectToAction("notfound");
                ctx.Resumes.FirstOrDefault(x => x.Id == id).Status = "DEINED";
                ctx.SaveChanges();
                return PartialView("ResumeList", GetResumeList(jid));
            }


        }
        public ActionResult SendResume(int id)
        {

            using (ApplicationDbContext ctx = new ApplicationDbContext())
            {


                string uid = User.Identity.GetUserId();
                var u = UserManager.FindById(uid);
                var resum = ctx.Resumes.Include("job").Include("own").FirstOrDefault(x => x.own.Id == u.Id && x.job.ID == id);
                if (resum==null)
                {
                    ctx.Resumes.Add(new Resume() {
                        job = ctx.Jobs.FirstOrDefault(x => x.ID == id),
                        path = u.ResumePath,
                        own = ctx.Users.FirstOrDefault(x=>x.Id==uid),
                        Status = "PROCESSING",
                        TimeSend = DateTime.Now,
                        TimeAnswer = DateTime.Now
          
                    });
                    ctx.SaveChanges();
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    ctx.Resumes.Remove(ctx.Resumes.Include("own").FirstOrDefault(x=>x.own.Id==uid));
                    ctx.SaveChanges();
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            }
        }
        ResumeListModel GetResumeList(int id)
        {
            ResumeListModel model = new ResumeListModel();
            using (ApplicationDbContext ctx = new ApplicationDbContext())
            {
                model.list = ctx.Resumes.Include("job").Include("own").Where(x => x.job.ID == id&& x.Status== "PROCESSING").ToList();
            }
            return model;
        }
        public ActionResult GetResumes(int id)
        {
         
            return PartialView("ResumeList", GetResumeList(id));
        }
        public ActionResult SubscribeManager(int id)
        {

            using (ApplicationDbContext ctx = new ApplicationDbContext())
            {

               
                string uid = User.Identity.GetUserId();
                if (ctx.Jobs.Include("subscribers").FirstOrDefault(x=>x.ID== id).subscribers.FirstOrDefault(x=>x.Id== uid) ==null)
                {
                    ctx.Jobs.Include("subscribers").FirstOrDefault(x => x.ID == id).subscribers.Add(ctx.Users.FirstOrDefault(x => x.Id == uid));
                    ctx.SaveChanges();
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    ctx.Jobs.Include("subscribers").FirstOrDefault(x => x.ID == id).subscribers.Remove(ctx.Users.FirstOrDefault(x => x.Id == uid));
                    ctx.SaveChanges();
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            }
        }
    }
}
