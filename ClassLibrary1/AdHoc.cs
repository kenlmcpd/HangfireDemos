using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;

namespace ConsoleApplication1
{
    [AutomaticRetry(Attempts = 3, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
    public class AdHoc
    {
        [Queue("adhoc")]
        [DisplayName("Adhoc-Enqueued")]
        public static void EnqueuedJob()
        {
            Console.WriteLine("Starting Long Running Job in adhoc queue");
            Thread.Sleep(120000);
        }

        [Queue("adhoc")]
        [DisplayName("Adhoc-Delayed")]
        public static void DelayedJob()
        {
            Console.WriteLine("Starting Delayed Running Job in adhoc queue");
            Thread.Sleep(120000);
        }
    }
}
