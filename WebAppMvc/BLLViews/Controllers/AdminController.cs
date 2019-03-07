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
        [HttpPost]
        public ActionResult BanUser(PartialBanModel model)
        {
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

          
            Repos<ApplicationUser> repos = new Repos<ApplicationUser>();
            IndexModel model1 = new IndexModel();
            model1.users = repos.ReadAll();
            model1.partialBanModel = new PartialBanModel();
            model1.partialRolesModel = new PartialRolesModel();
            return View("Index", model1);
        }

        [HttpPost]
        public ActionResult UpdateRoles(PartialRolesModel model)
        {
            if (model.IsAdmin)
                UserManager.AddToRole(model.UserID, "Admin");
            else
                UserManager.RemoveFromRole(model.UserID, "Admin");

            if (model.IsSupport)
                UserManager.AddToRole(model.UserID, "Support");
            else
                UserManager.RemoveFromRole(model.UserID, "Support");


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
     
        public async System.Threading.Tasks.Task<ActionResult> UnBan(string id)
        {
            if (!User.IsInRole("Admin"))
                return RedirectToAction("NotFound", "Home");

            
            Repos<BansList> repos = new Repos<BansList>();
            var user = await UserManager.FindByIdAsync(id);
            if(user==null)
                return RedirectToAction("NotFound", "Home");
      
            UserManager.RemoveFromRole(user.Id, "Banned");
            UserManager.AddToRole(user.Id, "User");
            UserManager.FindById(user.Id).Ban=null;
            //Repos<ApplicationUser> rUser = new Repos<ApplicationUser>();
            //user.Ban = null;
            //rUser.Update(user);
            return new EmptyResult();
        }
    }
}