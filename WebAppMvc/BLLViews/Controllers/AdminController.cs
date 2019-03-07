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
        // GET: Admin
        [Authorize]
        public ActionResult Index()
        {
            if(!User.IsInRole("Admin"))
                return RedirectToAction("NotFound", "Home");
            Repos<ApplicationUser> repos = new Repos<ApplicationUser>();
            IndexModel model = new IndexModel();
            model.users = repos.ReadAll();
            model.partialBanModel = new PartialBanModel();
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

            return new EmptyResult();
        }
    }
}