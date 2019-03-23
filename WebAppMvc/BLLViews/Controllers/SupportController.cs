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
    public class SupportController : Controller
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
        [Authorize(Roles = "Support")]
        public ActionResult AdmTicket(int id)
        {
            using (ApplicationDbContext ctx = new ApplicationDbContext())
            {
                
                AdmTicketModel model = new AdmTicketModel();
                model.ticket = ctx.Tickets.FirstOrDefault(x => x.ID == id);
                model.ticketMSGs = ctx.TicketMSGs.Where(x => x.ticket == ctx.Tickets.FirstOrDefault(y => y.ID == id)).ToList();
                model.mSG = new TicketMSG();
                model.listStatus.Add(new SelectListItem() { Value = "Closed", Text = "Closed" });
                model.listStatus.Add(new SelectListItem() { Value = "In process", Text = "In process" });
                model.listStatus.Add(new SelectListItem() { Value = "Need information", Text = "Need information" });
                model.uid = User.Identity.GetUserId();
                return View(model);
            }
        }
        [HttpPost]
        [Authorize(Roles = "Support")]
        public ActionResult AdmTicket(AdmTicketModel model)
        {
            using (ApplicationDbContext ctx = new ApplicationDbContext())
            {
                ctx.Tickets.FirstOrDefault(x => x.ID == model.ticket.ID).ticketMSGs.Add(new TicketMSG() { Text = $"Hello {ctx.Users.FirstOrDefault(x => x.Id == ctx.Tickets.FirstOrDefault(y => y.ID == model.ticket.ID).OwnerID).Email} \n<br/> {model.mSG.Text} \n<br/> Your hero with unlimied possibilities \n<br/> {User.Identity.GetUserName()}.", UserID = model.uid, Date = DateTime.Now });
                ctx.Tickets.FirstOrDefault(x => x.ID == model.ticket.ID).LastUpdate = DateTime.Now;
                ctx.Tickets.FirstOrDefault(x => x.ID == model.ticket.ID).Status = model.newStatus;
                UserManager.SendEmail(ctx.Tickets.FirstOrDefault(x => x.ID == model.ticket.ID).OwnerID, "Support",  $"Your request [ID: {model.ticket.ID}] has been answered!");

                ctx.SaveChanges();
            }
            return Redirect("~/support/tickets");
        }
        [Authorize]
        public ActionResult Ticket(int id)
        {
            using (ApplicationDbContext ctx = new ApplicationDbContext())
            {
                TicketModel model = new TicketModel();
                var userId = User.Identity.GetUserId();
                if (ctx.Tickets.FirstOrDefault(x => x.OwnerID == userId && x.ID == id) == null)
                    return RedirectToAction("NotFound", "Home");
                model.ticket = ctx.Tickets.FirstOrDefault(x => x.ID == id);
                model.ticketMSGs = ctx.TicketMSGs.Where(x => x.ticket == ctx.Tickets.FirstOrDefault(y => y.ID == id && y.OwnerID == userId)).ToList();
                model.mSG = new TicketMSG();
                model.uid = User.Identity.GetUserId();
                return View(model);
            }
        }
        [HttpPost]
        [Authorize]
        public ActionResult Ticket(TicketModel model)
        {
            using (ApplicationDbContext ctx = new ApplicationDbContext())
            {
                ctx.Tickets.FirstOrDefault(x => x.ID == model.ticket.ID).ticketMSGs.Add(new TicketMSG() { Text =model.mSG.Text, UserID = User.Identity.GetUserId(), Date = DateTime.Now });
                ctx.Tickets.FirstOrDefault(x => x.ID == model.ticket.ID).LastUpdate = DateTime.Now;
                ctx.SaveChanges();
            }
            return Redirect("~/support/ticket/" + model.ticket.ID);
        }
        [Authorize]
        public ActionResult MyTickets()
        {
            using (ApplicationDbContext ctx = new ApplicationDbContext())
            {
                MyTicketsModel model = new MyTicketsModel();
                string id = User.Identity.GetUserId();
                model.tickets = ctx.Tickets.Where(x => x.OwnerID == id).ToList();

                model.IsSupport = UserManager.IsInRole(User.Identity.GetUserId(), "Support");
                return View(model);
            }

        }
        [Authorize(Roles = "Support")]
        public ActionResult PanelControl()
        {
            using (ApplicationDbContext ctx = new ApplicationDbContext())
            {
                BanControlModels model = new BanControlModels();
                model.Ban = new BansList();
                return View(model);
            }

        }
        [Authorize(Roles = "Support")]
        public ActionResult Tickets()
        {
            using (ApplicationDbContext ctx = new ApplicationDbContext())
            {
                TicketsModel model = new TicketsModel();
                model.tickets = ctx.Tickets.Where(x => x.Status != "Closed").ToList();
                model.uid = User.Identity.GetUserId();
                model.tickets = model.tickets.OrderBy(o => o.LastUpdate).ToList();
                return View(model);
            }

        }
        [Authorize]
        public ActionResult NewTicket()
        {
            NewTicketModel model = new NewTicketModel();
            model.mSG = new TicketMSG();
            model.ticket = new Ticket();
            return View(model);
        }
        [HttpPost]
        [Authorize]
        public ActionResult NewTicket(NewTicketModel model)
        {
            using (ApplicationDbContext ctx = new ApplicationDbContext())
            {
                var ticket = new Ticket()
                {
                    LastUpdate = DateTime.Now,
                    Status = "Open",
                    OwnerID = User.Identity.GetUserId(),
                    Theme = model.ticket.Theme,
                };
                ctx.TicketMSGs.Add(new TicketMSG() { ticket = ticket, Text = model.mSG.Text, Date = DateTime.Now, UserID = User.Identity.GetUserId() });
                UserManager.SendEmail(User.Identity.GetUserId(), "We received a request from you", "We received a request from you!");
                ctx.SaveChanges();
            }
            return Redirect("~/support/mytickets");
        }
        [Authorize(Roles = "Support")]
        [HttpPost]
        public ActionResult PanelControl(BanControlModels model)
        {
            if (!ModelState.IsValid)
                return View(model);
            ApplicationUser user = null;
           
            if (model.IdOrEmail.Contains("@"))
                user = UserManager.FindByEmail(model.IdOrEmail);
            else
                user = UserManager.FindById(model.IdOrEmail);

            if (model.BanOrUnban==false)
            {
                UserManager.FindById(user.Id).Ban = null;
                UserManager.RemoveFromRole(user.Id, "Banned");
                UserManager.AddToRole(user.Id, "User");
                UserManager.SendEmail(user.Id, "Your account has been unbanned. Time:  " + DateTime.Now,$"Hello {user.UserName} Your account has been unbanned!!!");
                BanControlModels models = new BanControlModels();
                models.Ban = new BansList();
                return View(models);
            }
            else
            {
                if (model.Ban.IsPermanent)
                {
                    UserManager.FindById(user.Id).Ban = new BansList() { IsPermanent = true, Reason = model.Ban.Reason, DateBan = DateTime.Now, ToDate=DateTime.Now.AddYears(120) };
                    UserManager.RemoveFromRole(user.Id, "User");
                    UserManager.AddToRole(user.Id, "Banned");
                    UserManager.SendEmail(user.Id, "Your account has been banned. Time:  " + DateTime.Now, $"Hello {user.UserName} Your account has been banned!!!");

                }
                else
                {
                    UserManager.FindById(user.Id).Ban = new BansList() { IsPermanent = false, ToDate= DateTime.Now.AddHours(model.CountHour), Reason = model.Ban.Reason, DateBan = DateTime.Now};
                    UserManager.RemoveFromRole(user.Id, "User");
                    UserManager.AddToRole(user.Id, "Banned");
                    UserManager.SendEmail(user.Id, "Your account has been banned. Time:  " + DateTime.Now, $"Hello {user.UserName} Your account has been banned!!!");
                }
                BanControlModels models = new BanControlModels();
                models.Ban = new BansList();
                return View(models);
            }
        }

    }
}