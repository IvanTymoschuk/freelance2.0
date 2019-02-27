using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace BLLViews.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {

        public string FullName { get; set; }
        public City City { get; set; }
        public string AvaPath { get; set; }
        public double Raiting { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            return userIdentity;
        }
    }
    public class City
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class Category
    {
        public Category()
        {
            Jobs = new List<Jobs>();
        }
        public int ID { get; set; }
        public string Name { get; set; }
        public ICollection<Jobs> Jobs { get; set; }
    }
    public class Jobs
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public decimal Salary { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public City City { get; set; }

        public Category Category { get; set; }
        public ApplicationUser UserOwner { get; set; }
    }
    public class Ticket
    {
        public Ticket()
        {
            ticketMSGs = new List<TicketMSG>();
        }
        public int ID { get; set; }
        public string Status { get; set; }
        public virtual ApplicationUser Owner { get; set; }
        public virtual ICollection<TicketMSG> ticketMSGs { get; set; }
        public DateTime LastUpdate { get; set; }
        public string Theme { get; set; }
    }

    public class TicketMSG
    {
        public int ID { get; set; }
        public string Text { get; set; }
        public virtual Ticket ticket { get; set; }
        public ApplicationUser User { get; set; }
        public DateTime Date { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }
        public virtual DbSet<Jobs> Jobs { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<City> Cities { get; set; }
        public virtual DbSet<Ticket> Tickets { get; set; }
        public virtual DbSet<TicketMSG> TicketMSGs { get; set; }
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
   
    }
    class MyContextInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext db)
        {

            db.SaveChanges();
        }
    }
}