using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.Common;
using Hangfire.SqlServer;
using Owin;

namespace ConsoleApplication1
{
    public class Startup
    {
        public BackgroundJobServer Server { get; set; }

        public void Configuration(IAppBuilder app)
        {
            var storage = new SqlServerStorage(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            storage.GetMonitoringApi();

            var options = new BackgroundJobServerOptions
            {
                Queues = new[] { "adhoc" }
            };

            Server = new BackgroundJobServer(options, storage);

            //var jobManager = new RecurringJobManager(storage);
            //var job = Job.FromExpression(() => AdHoc.EnqueuedJob());
            //jobManager.AddOrUpdate("test", job, Cron.Daily(), TimeZoneInfo.Utc, "adhoc");
        }
    }
}
