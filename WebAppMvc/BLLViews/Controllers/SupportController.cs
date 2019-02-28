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
            [Authorize(Roles ="Support")]
            public ActionResult AdmTicket(string id, int ticket)
            {
                using (ApplicationDbContext ctx = new ApplicationDbContext())
                {
                    AdmTicketModel model = new AdmTicketModel();
                    model.ticket = ctx.Tickets.FirstOrDefault(x => x.ID == ticket);
                    model.uid = id;
                    model.ticketMSGs = ctx.TicketMSGs.Where(x => x.ticket == ctx.Tickets.FirstOrDefault(y => y.ID == ticket)).ToList();
                    model.mSG = new TicketMSG();
                    model.listStatus.Add(new SelectListItem() {  Value="Closed", Text="Closed"});
                    model.listStatus.Add(new SelectListItem() { Value = "In process", Text = "In process" });
                    model.listStatus.Add(new SelectListItem() { Value = "Need information", Text = "Need information" });
                
                     return View(model);
                }
            }
            [HttpPost]
            [Authorize(Roles = "Support")]
            public ActionResult AdmTicket(AdmTicketModel model)
                {
                    using (ApplicationDbContext ctx = new ApplicationDbContext())
                    {
                        ctx.Tickets.FirstOrDefault(x => x.ID == model.ticket.ID).ticketMSGs.Add(new TicketMSG() { Text = model.mSG.Text, UserID = model.uid, Date = DateTime.Now });
                         ctx.Tickets.FirstOrDefault(x => x.ID == model.ticket.ID).LastUpdate = DateTime.Now;
                         ctx.Tickets.FirstOrDefault(x => x.ID == model.ticket.ID).Status = model.newStatus;
                        UserManager.SendEmail(ctx.Tickets.FirstOrDefault(x => x.ID == model.ticket.ID).OwnerID, "Support", "Your request has been answered!");
                       
                       ctx.SaveChanges();
                    }
                     return Redirect("~/support/tickets/" + model.uid);
                 }
            [Authorize]
            public ActionResult Ticket(string id, int ticket)
            {
                using (ApplicationDbContext ctx = new ApplicationDbContext())
                {
                    TicketModel model = new TicketModel();
                    if (ctx.Tickets.FirstOrDefault(x => x.OwnerID == id && x.ID == ticket) == null)
                        return HttpNotFound("Ticket not found");
                    model.ticket = ctx.Tickets.FirstOrDefault(x => x.ID == ticket);
                    model.uid = id;
                    model.ticketMSGs = ctx.TicketMSGs.Where(x => x.ticket == ctx.Tickets.FirstOrDefault(y => y.ID == ticket && y.OwnerID==id)).ToList();
                    model.mSG = new TicketMSG();
                    return View(model);
                }
            }
            [HttpPost]
            [Authorize]
            public ActionResult Ticket(TicketModel model)
            {
                using (ApplicationDbContext ctx = new ApplicationDbContext())
                {
                ctx.Tickets.FirstOrDefault(x => x.ID == model.ticket.ID).ticketMSGs.Add(new TicketMSG() { Text = model.mSG.Text, UserID = model.uid, Date = DateTime.Now });
                ctx.SaveChanges();
                }
            return Redirect("~/support/ticket/" + model.uid + "/" + model.ticket.ID);
            }
            [Authorize]
            public ActionResult MyTickets(string id)
            {
                using (ApplicationDbContext ctx = new ApplicationDbContext())
                {
                    MyTicketsModel model = new MyTicketsModel();
                    model.tickets = ctx.Tickets.Where(x => x.OwnerID == id).ToList();
                    model.uid = id;

                model.IsSupport = UserManager.IsInRole(id, "Support");
                return View(model);
                }

            }
            [Authorize(Roles = "Support")]
            public ActionResult Tickets(string id)
            {
                using (ApplicationDbContext ctx = new ApplicationDbContext())
                {
                    TicketsModel model = new TicketsModel();
                    model.tickets = ctx.Tickets.Where(x=> x.Status!="Closed").ToList();
                    model.uid = id;
                    model.tickets = model.tickets.OrderBy(o => o.LastUpdate).ToList();
                    return View(model);
                }

            }
        [Authorize]
            public ActionResult NewTicket(string id)
            {
                NewTicketModel model = new NewTicketModel();
                model.mSG = new TicketMSG();
                model.ticket = new Ticket();
                model.uid = id;
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
                    OwnerID = model.uid,
                    Theme = model.ticket.Theme,
                    };
                    ctx.TicketMSGs.Add(new TicketMSG() { ticket = ticket, Text = model.mSG.Text, Date = DateTime.Now, UserID = model.uid});
                    ctx.SaveChanges();
                }
                return Redirect("~/support/mytickets/" + model.uid);
            }
       
    }
}