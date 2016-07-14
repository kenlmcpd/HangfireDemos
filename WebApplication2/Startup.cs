using Microsoft.Owin;
using Owin;
using WebApplication2.App_Start;

[assembly: OwinStartupAttribute("AuthenticationDemo",typeof(WebApplication2.Startup))]
namespace WebApplication2
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
