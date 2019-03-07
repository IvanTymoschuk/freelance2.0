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
    }
    public class PartialBanModel
    {
        public BansList Ban { get; set; }
        public string UserId { get; set; }
        public int CountHour { get; set; }
    }
}