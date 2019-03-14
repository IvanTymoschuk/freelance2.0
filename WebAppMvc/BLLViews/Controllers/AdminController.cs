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
            if (!User.IsInRole("Admin") || !User.IsInRole("Support"))
                return RedirectToAction("NotFound", "Home");
            var user = UserManager.FindById(model.UserId);

            if (model.Ban.IsPermanent)
            {
                UserManager.FindById(user.Id).Ban = new BansList() { IsPermanent = true, Reason = model.Ban.Reason, DateBan = DateTime.Now, ToDate = DateTime.Now.AddYears(120) };
                UserManager.RemoveFromRole(user.Id, "User");
                UserManager.AddToRole(user.Id, "Banned");
                UserManager.SendEmail(user.Id, "Your account has been banned. Time:  " + DateTime.Now, $"Hello {user.UserName} Your account has been banned!!!");

            }
            else
            {
                UserManager.FindById(user.Id).Ban = new BansList() { IsPermanent = false, ToDate = DateTime.Now.AddHours(model.CountHour), Reason = model.Ban.Reason, DateBan = DateTime.Now };
                UserManager.RemoveFromRole(user.Id, "User");
                UserManager.AddToRole(user.Id, "Banned");
                UserManager.SendEmail(user.Id, "Your account has been banned. Time:  " + DateTime.Now, $"Hello {user.UserName} Your account has been banned!!!");
            }

            ICollection<GetAllUsers> model1 = new List<GetAllUsers>();
            Repos<ApplicationUser> repos = new Repos<ApplicationUser>();
            var list = repos.ReadAll();
            foreach (var el in list)
            {
                GetAllUsers u = new GetAllUsers();
                u.user = el;
                if (el.Ban != null)
                {
                    u.ban = el.Ban;

                    if (el.Ban.IsPermanent || el.Ban.ToDate > DateTime.Now)
                        u.isBanned = true;
                }
                model1.Add(u);
            }
            return PartialView("_Users", model1);
        }

        [HttpPost]
        [Authorize]
        public ActionResult UpdateRoles(PartialRolesModel model)
        {
            if (!User.IsInRole("Admin"))
                return RedirectToAction("NotFound", "Home");
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
            if (!User.IsInRole("Admin") || !User.IsInRole("Support"))
                return RedirectToAction("NotFound", "Home");

            

            var user = UserManager.FindById(id);
            if(user==null)
                return RedirectToAction("NotFound", "Home");
      
            UserManager.RemoveFromRole(user.Id, "Banned");
            UserManager.AddToRole(user.Id, "User");
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.Users.FirstOrDefault(x => x.Id == user.Id).Ban.ToDate = DateTime.Now;
                db.Users.FirstOrDefault(x => x.Id == user.Id).Ban.IsPermanent= false;
                db.SaveChanges();
            }

            ICollection<GetAllUsers> model = new List<GetAllUsers>();
            Repos<ApplicationUser> repos = new Repos<ApplicationUser>();
            var list = repos.ReadAll();
            foreach (var el in list)
            {
                GetAllUsers u = new GetAllUsers();
                u.user = el;
                if (el.Ban != null)
                {
                    u.ban = el.Ban;

                    if (el.Ban.IsPermanent || el.Ban.ToDate > DateTime.Now)
                        u.isBanned = true;
                }
                model.Add(u);
            }
            return PartialView("_Users", model);
        }
        [Authorize]
        public ActionResult GetUsers()
        {
            if (!User.IsInRole("Admin"))
                return RedirectToAction("NotFound", "Home");
            ICollection<GetAllUsers> model = new List<GetAllUsers>();
            Repos<ApplicationUser> repos = new Repos<ApplicationUser>();
            var list = repos.ReadAll();
            foreach (var el in list)
            {
                GetAllUsers u = new GetAllUsers();
                u.user = el;
                if (el.Ban != null)
                {
                    u.ban = el.Ban;

                    if (el.Ban.IsPermanent || el.Ban.ToDate > DateTime.Now)
                        u.isBanned = true;
                    else
                        u.isBanned = false;
                }
                model.Add(u);
            }
            return PartialView("_Users", model);
        }
    }
}