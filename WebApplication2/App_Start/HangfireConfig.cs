using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.Dashboard;
using Owin;

namespace WebApplication2.App_Start
{
    public class HangfireConfig
    {
        public static void ConfigureHangfire(IAppBuilder app)
        {
            GlobalConfiguration.Configuration
                .UseSqlServerStorage("DefaultConnection");

            app.UseHangfireDashboard("/jobs", new DashboardOptions
            {
                AuthorizationFilters = new[]
                {
                    new Hangfire.Dashboard.BasicAuthAuthorizationFilter(new Hangfire.Dashboard.BasicAuthAuthorizationFilterOptions
                    {
                        SslRedirect = false,
                        LoginCaseSensitive = true,
                        RequireSsl = false,
                        Users = new List<BasicAuthAuthorizationUser>()
                        {
                            new BasicAuthAuthorizationUser
                            {
                                Login = "HangfireUser",
                                PasswordClear = "HangfirePassword"
                            }
                        }
                    }
                )}
            });


            app.UseHangfireServer();
        }
    }
}
