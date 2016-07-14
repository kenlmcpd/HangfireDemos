using System.Collections.Generic;
using Hangfire;
using Hangfire.Dashboard;
using Owin;
using WebApplication4.Jobs;

namespace WebApplication4
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
            
            RecurringJob.AddOrUpdate("longrunning", () => Recurring.LongRunningJob(), Cron.Hourly);

            RecurringJob.AddOrUpdate("crashing", () => Recurring.ThrowingJob(), Cron.Daily);
        }
    }
}
