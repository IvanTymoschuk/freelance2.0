using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BLLViews.Startup))]
namespace BLLViews
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
