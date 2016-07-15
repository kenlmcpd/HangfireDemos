# HangfireDemos
Examples of Hangfire

This is an example of how to set up Hangfire starting out with basic techniques flowing up to advanced Owin Hosted service with multiple queues.

The solution is set up with 7 different folders. Each folder houses the compled code for this demo


1) Basic Setup
	Create asp.net web application
	Select MVC with Individual User Accounts Authorization (doing this so Entity Framework is added)
	
	Start the MVC App and register a user so that the database is created

	Add the following NuGet Packages
		Hangfire
		Hangfire.Core
		Hangfire.SqlServer

	Copy the information out of the Hangfire readme into Startup.cs

		GlobalConfiguration.Configuration.UseSqlServerStorage("<name or connection string>");
		app.UseHangfireDashboard();
		app.UseHangfireServer();

	Open the web config and get the name of the your database connection string

	overwrite the <name or connection string> with it  ie. DefaultConnection

	Start the MVC App again

	go to /hangfire

	Explore the dashboard
	
	Also Explore the SQL database, Hangfire automatically created the needed tables.


2) Add Authorization and a SQL database  

	Create an empty database - you can do it in Visual Studio by opening SQL Server Object Explorer and right clicking one of your SQL servers.
	Example:
		Data Source=(localdb)\ProjectsV13;Initial Catalog=HangfireDemo;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False

	Create asp.net web application
		Select MVC with Individual User Accounts Authorization - This is just so we don't have to add other NuGet Packages
	
	Change the connect string to match the database you created
	
	Add the following NuGet Packages
		Hangfire
		Hangfire.Core
 		Hangfire.Dashboard.Authorization
		Hangfire.SqlServer
		
	In the App_Start folder create a class named HangfireConfig.cs

	Use the following the code:

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

	Add the following to Startup.cs
		HangfireConfig.ConfigureHangfire(app);

	Comment out 
		// ConfigureAuth(app); - This is because we are using Basic Auth.

	Start the MVC App

	Navigate to  /jobs - Notice we set this with app.UseHangfireDashboard("/jobs"  where in the first demo it was the default hangfire.

	Log in with HangfireUser and HangfirePassword

3) Add a Recurring Jobs - these are scheduled jobs that run in the background.

	Create a folder name Jobs

	Add a class to the folder named Recurring.cs

	Use the following code:

		public class Recurring
		{
			public static void LongRunningJob()
			{
				Thread.Sleep(120000);
			}

			public static void ThrowingJob()
			{
				Thread.Sleep(10000);
				throw new ApplicationException("OMG");
			}
		}

	At the bottom of the ConfigureHangfire Add this code

		    RecurringJob.AddOrUpdate("longrunning", () => Recurring.LongRunningJob(), Cron.Hourly);

        RecurringJob.AddOrUpdate("crashing", () => Recurring.ThrowingJob(), Cron.Daily);

	Start the MVC App

	Go to /Jobs

	See the recurring jobs are show under recuring 

	Kick off the long running job

	Wait a little bit Kick off the Crashing job

	Allow long running to finish

	After the crashing gets scheduled a couple times, delete it

4) Advanced Configuration

	Above public class Recurring Add the following attribute

		[AutomaticRetry(Attempts = 3, OnAttemptsExceeded = AttemptsExceededAction.Delete)]

	Above the jobs methods add following attributes
		
		[DisplayName("Long Running")]
		[DisplayName("Crashing Job")]

	Start the MVC App

	Go to /Jobs

	See the recurring jobs are there but now with friendly names. The Crashing Job will try to run 3 times and then delete.

	Kick off the long running job

	Allow long running to finish

5)  AdHoc Jobs - these are background jobs that you kick off at will. 

	In the Jobs folder, Create a class named AdHoc

	Add the following code

	[AutomaticRetry(Attempts = 3, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
    public class AdHoc
    {
        [DisplayName("Adhoc-Enqueued")]
        public static void EnqueuedJob()
        {
            Thread.Sleep(120000);
        }

        [DisplayName("Adhoc-Delayed")]
        public static void DelayedJob()
        {
            Thread.Sleep(120000);
        }
    }

	Open the Home Controller

	Add this code to either About or Contact

		    BackgroundJob.Enqueue(() => AdHoc.EnqueuedJob());

        BackgroundJob.Schedule(() => AdHoc.EnqueuedJob(), TimeSpan.FromMinutes(1));


		Start the MVC App

		Open another browser window with the same localhost:xxxx and go to  /Jobs

		We should see our previous jobs in the dashboard

		We still have our 2 recurring jobs,

		Go to either about or contact to enqueue the jobs..

		Go back to Hangfire, we should see 1 enqueued and 1 scheduled

		Wait for them to run

6) Queues - this will help you distribute load accross multiple machines.
	
	Open the Home Controller

	Change the Background code to 

    var jobClient = new BackgroundJobClient();
    var enqueuedState = new EnqueuedState("adhoc");
            
    jobClient.Create(() => AdHoc.EnqueuedJob(), enqueuedState);
    jobClient.Schedule(() => AdHoc.DelayedJob(), TimeSpan.FromMinutes(1));


            
	 Start the MVC App

	 Go to either about or contact to enqueue the jobs..

	 Enqueue the jobs - they never run; they just stay in queue. This is because we do not have an adhoc queue.

7) Queues Part 2

	Comment out the AdHoc class in the MVC application

	 Create a ClassLibrary and also create a console application
	 
	 Add the following NuGet Packages to the console app
		Microsoft.Owin
		Microsoft.Owin.Host.SystemWeb
		Microsoft.Owin.Hosting
		Microsoft.Owin.Diagnostics
		Microsoft.Owin.Host.HttpListener
		Microsoft.Owin.SelfHost - The read me shows you what to do next

	Note:
		If you want to make this a windows service, install TopShelf

	Add the following NuGet Packages to both the console and the ClassLibrary:
		Hangfire
		Hangfire.Core
		Hangfire.SqlServer

	In the Program Class add the following code to the Main Method

		static void Main(string[] args)
        {
            using (WebApp.Start<Startup>("http://localhost:12345"))
            {
                Console.ReadLine();
            }
        }

	Copy your connection String out of the MVC Web.config, Add it to the App.Config

	Add the System.Configuration Reference to the project

	Create a class named Startup.cs

	Add the following code

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
			}
		}

		Create a class named AdHoc

		Add the following code

	[Queue("adhoc")]
    [AutomaticRetry(Attempts = 3, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
    public class AdHoc
    {
        [DisplayName("Adhoc-Enqueued")]
        public static void EnqueuedJob()
        {
            Console.WriteLine("Starting Long Running Job in adhoc queue");
            Thread.Sleep(120000);
        }

        [DisplayName("Adhoc-Delayed")]
        public static void DelayedJob()
        {
            Console.WriteLine("Starting Delayed Running Job in adhoc queue");
            Thread.Sleep(120000);
        }
    }

	Add a reference to the ClassLibrary in both the MVC app and the Console app.
	
	Set Both the MVC project and the console projct to run

	Go to /jobs/servers

	Notice the adhoc queue now
	
	Run the adhoc jobs again by opening the home/contact or about
	
	Watch the dashboard and the console as the jobs run in the adhoc queue that is hosted by the console.
	
	







