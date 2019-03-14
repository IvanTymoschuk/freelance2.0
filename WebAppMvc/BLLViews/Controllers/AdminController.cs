using BLLViews.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
namespace BLLViews.Controllers
{
    public class AdminController : Controller
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
        [Authorize]
        public ActionResult GetRoles(string id)
        {
            if (!User.IsInRole("Admin"))
                return RedirectToAction("NotFound", "Home");

            // NO WORKING
            //PartialRolesModel model = new PartialRolesModel();
            //model.roles= UserManager.GetRoles(id);
            //model.UserID = id;
            //return PartialView("_RolePanel", model);

            //IndexModel model = new IndexModel();
            //model.partialBanModel = new PartialBanModel();
            //Repos<ApplicationUser> repos = new Repos<ApplicationUser>();
            //model.users = repos.ReadAll();
            //model.partialRolesModel = new PartialRolesModel();
            //model.partialRolesModel.roles= UserManager.GetRoles(id);
            //model.partialRolesModel.UserID = id;
            //if (UserManager.GetRoles(id).Contains("Admin"))
            //    model.partialRolesModel.IsAdmin = true;
            //if (UserManager.GetRoles(id).Contains("Support"))
            //    model.partialRolesModel.IsSupport = true;

            PartialRolesModel model = new PartialRolesModel();
         
            model.roles = UserManager.GetRoles(id);
            model.UserID = id;
            if (UserManager.GetRoles(id).Contains("Admin"))
                model.IsAdmin = true;
            if (UserManager.GetRoles(id).Contains("Support"))
                model.IsSupport = true;
            return PartialView("_RolePanel", model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult BanUser(PartialBanModel model)
        {
           
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var uid = model.UserId;
                if (model.Ban.IsPermanent)
                {

                    db.Users.FirstOrDefault(x => x.Id == uid).Ban = new BansList() { IsPermanent = true, Reason = model.Ban.Reason, DateBan = DateTime.Now, ToDate = DateTime.Now.AddYears(120) };
                    UserManager.RemoveFromRole(uid, "User");
                    UserManager.AddToRole(uid, "Banned");
                    UserManager.SendEmail(uid, "Your account has been banned. Time:  " + DateTime.Now, $"Hello {db.Users.FirstOrDefault(x => x.Id == uid).UserName} Your account has been banned!!!");
                    db.SaveChanges();

                }
                else
                {
                    db.Users.FirstOrDefault(x => x.Id == uid).Ban = new BansList() { IsPermanent = false, ToDate = DateTime.Now.AddHours(model.CountHour), Reason = model.Ban.Reason, DateBan = DateTime.Now };
                    UserManager.RemoveFromRole(uid, "User");
                    UserManager.AddToRole(uid, "Banned");
                    UserManager.SendEmail(uid, "Your account has been banned. Time:  " + DateTime.Now, $"Hello {db.Users.FirstOrDefault(x => x.Id == uid).UserName} Your account has been banned!!!");
                    db.SaveChanges();
                }
            }
            return PartialView("_Users", GetAllUsers());
        }

        [HttpPost]
        [Authorize]
        public ActionResult UpdateRoles(PartialRolesModel model)
        {
            var user = UserManager.FindById(model.UserID);
            if (user != null)
            {
                if(model.IsSupport==false&&UserManager.IsInRole(user.Id,"Support"))
                     UserManager.RemoveFromRole(user.Id, "Support");
                if(model.IsSupport == true && UserManager.IsInRole(user.Id, "Support")==false)
                    UserManager.AddToRole(user.Id, "Support");

                if (model.IsAdmin == false && UserManager.IsInRole(user.Id, "Admin"))
                    UserManager.RemoveFromRole(user.Id, "Admin");
                if (model.IsAdmin == true && UserManager.IsInRole(user.Id, "Admin") == false)
                    UserManager.AddToRole(user.Id, "Admin");
            }
            else
            {
                return RedirectToAction("NotFound", "Home");
            }


            return new EmptyResult();
        }

        [Authorize]
        public ActionResult Index()
        {
            if(!User.IsInRole("Admin"))
                return RedirectToAction("NotFound", "Home");
            Repos<ApplicationUser> repos = new Repos<ApplicationUser>();
            IndexModel model = new IndexModel();
            model.users = repos.ReadAll();
            model.partialBanModel = new PartialBanModel();
            model.partialRolesModel = new PartialRolesModel();
       
                return View(model);
        }
        [Authorize]
        public ActionResult UnBan(string id)
        {

                var u = UserManager.FindById(id);
                if (u == null)
                    return RedirectToAction("NotFound", "Home");

                UserManager.RemoveFromRole(u.Id, "Banned");
                UserManager.AddToRole(u.Id, "User");
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
   
                    db.Users.Include("Ban").FirstOrDefault(x => x.Id == id).Ban.ToDate = DateTime.Now;
                    db.Users.Include("Ban").FirstOrDefault(x => x.Id == id).Ban.IsPermanent = false;
                    db.SaveChanges();
                }
            


      
            return PartialView("_Users", GetAllUsers());
        }

        ICollection<GetAllUsers> GetAllUsers()
        {
            ICollection<GetAllUsers> model = new List<GetAllUsers>();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {

                var list = db.Users.Include("Ban").ToList();
                foreach (var el in list)
                {
                    GetAllUsers u = new GetAllUsers();
                    u.user = el;
                    if (el.Ban != null)
                    {
                        u.ban = el.Ban;

                        if (el.Ban.IsPermanent || el.Ban.ToDate > DateTime.Now || UserManager.IsInRole(el.Id, "Banned") == true)
                            u.isBanned = true;
                    }
                    model.Add(u);
                }
                return model;
            }
        }
        [Authorize]
        public ActionResult GetUsers()
        {
            if (!User.IsInRole("Admin"))
                return RedirectToAction("NotFound", "Home");
             
            return PartialView("_Users", GetAllUsers());
        }
    }
}