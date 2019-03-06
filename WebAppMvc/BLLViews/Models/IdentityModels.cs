using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using static BLLViews.Models.MyContextInitializer;

namespace BLLViews.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            Bets = new List<Job>();

        }
        public string FullName { get; set; }
        virtual public City City { get; set; }
        public string AvaPath { get; set; }
        public double Raiting { get; set; }
        virtual public List<Job> Bets { get; set; }
        virtual public BansList Ban { get; set; }
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
            Jobs = new List<Job>();
        }
        public int ID { get; set; }
        public string Name { get; set; }
        public ICollection<Job> Jobs { get; set; }
    }
    public class Job
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
        public string OwnerID { get; set; }
        public string Status { get; set; }
        public DateTime LastUpdate { get; set; }
        public string Theme { get; set; }
        virtual public ICollection<TicketMSG> ticketMSGs { get; set; }
    }
    public class BansList
    {
        public int ID { get; set; }
        public string Reason { get; set; }
        public DateTime ToDate { get; set; }
        public DateTime DateBan { get; set; }
        public bool IsPermanent { get; set; }
    }

    public class TicketMSG
    {
        public int ID { get; set; }
        public string Text { get; set; }
        public virtual Ticket ticket { get; set; }
        public string UserID { get; set; }
        public DateTime Date { get; set; }
    }

   
        public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
        {
            public ApplicationDbContext()
                : base("DefaultConnection", throwIfV1Schema: false)
            {
            }
            public virtual DbSet<Job> Jobs { get; set; }
            public virtual DbSet<Category> Categories { get; set; }
            public virtual DbSet<City> Cities { get; set; }
            public virtual DbSet<BansList> Bans { get; set; }
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
            //Andriy
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
            var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(db));

            var role1 = new IdentityRole { Name = "Admin" };
            var role2 = new IdentityRole { Name = "User" };
            var role3 = new IdentityRole { Name = "Support" };
            var role4 = new IdentityRole { Name = "Banned" };
            var support = new ApplicationUser { Email = "Support@freelance.localhost", UserName = "Support@freelance.localhost"};
            string password = "Test228";
            support.EmailConfirmed = true;
            var result = userManager.Create(support, password);
            roleManager.Create(role1);
            roleManager.Create(role2);
            roleManager.Create(role3);
            roleManager.Create(role4);
            if (result.Succeeded)
            {
                userManager.AddToRole(support.Id, role3.Name);
                userManager.AddToRole(support.Id, role4.Name);
            }

           
            //Ivan
            var user1 = new ApplicationUser { Email = "lox@mail.com",UserName= "lox@mail.com", FullName="Admin lox", Raiting = 4};
            user1.EmailConfirmed = true;
            var res = userManager.Create(user1, "Ii111111");
            if (res.Succeeded)
            {
                userManager.AddToRole(user1.Id, role2.Name);

                db.Jobs.Add(new Job()
                {
                    Name = "Make clean",
                    Category = new Category() { Name = "Home" },
                    City = new City() { Name = "Rivne" },
                    Date = DateTime.Now,
                    Description = "blabla",
                    Salary = 1200,
                    UserOwner = user1,
                });
            }



            base.Seed(db);
        }
    }

}
