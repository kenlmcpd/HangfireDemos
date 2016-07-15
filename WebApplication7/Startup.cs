using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebApplication7.Startup))]
namespace WebApplication7
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //ConfigureAuth(app);
            HangfireConfig.ConfigureHangfire(app);
        }
    }
}
