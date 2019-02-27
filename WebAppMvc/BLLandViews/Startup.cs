using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BLLandViews.Startup))]
namespace BLLandViews
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
