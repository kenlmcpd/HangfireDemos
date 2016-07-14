using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace WebApplication3.Jobs
{
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
}