﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Hangfire;
using Hangfire.States;
using WebApplication6.Jobs;

namespace WebApplication6.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            var jobClient = new BackgroundJobClient();
            var enqueuedState = new EnqueuedState("adhoc");

            jobClient.Create(() => AdHoc.EnqueuedJob(), enqueuedState);
            jobClient.Schedule(() => AdHoc.DelayedJob(), TimeSpan.FromMinutes(1));

            return View();
        }
    }
}