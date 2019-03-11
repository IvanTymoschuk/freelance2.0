using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BLLViews.Models
{
    public class IndexModel
    {
        public IndexModel()
        {
            users = new HashSet<ApplicationUser>();
        }
        public ICollection<ApplicationUser> users { get; set; }
        public PartialBanModel partialBanModel { get; set; }
        public PartialRolesModel partialRolesModel { get; set; }
    }
    public class GetAllUsers
    {
        public ApplicationUser user { get; set; }
        public BansList ban { get; set; }
        public bool isBanned { get; set; }
    }
    public class PartialBanModel
    {
        public BansList Ban { get; set; }
        public string UserId { get; set; }
        public int CountHour { get; set; }
    }
    public class PartialRolesModel
    {
        public PartialRolesModel()
        {
            roles = new List<String>();
        }
        public bool IsAdmin { get; set; }
        public bool IsSupport { get; set; }
        public string UserID { get; set; }
        public ICollection<string> roles { get; set; }
    }
}