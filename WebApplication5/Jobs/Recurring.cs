using System;
using System.ComponentModel;
using System.Threading;
using Hangfire;

namespace WebApplication5.Jobs
{
    [AutomaticRetry(Attempts = 3, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
    public class Recurring
    {
        [DisplayName("Long Running")]
        public static void LongRunningJob()
        {
            Thread.Sleep(120000);
        }

        [DisplayName("Crashing Job")]
        public static void ThrowingJob()
        {
            Thread.Sleep(10000);
            throw new ApplicationException("OMG");
        }
    }
}