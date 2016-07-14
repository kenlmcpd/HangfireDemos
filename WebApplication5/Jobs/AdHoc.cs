using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;

namespace WebApplication5.Jobs
{
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
}
