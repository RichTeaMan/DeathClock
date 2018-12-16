using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DeathClock.Web.UI.Models;

namespace DeathClock.Web.UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataContext dataContext;

        public HomeController(DataContext dataContext)
        {
            this.dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
        }

        public IActionResult Index()
        {
            var model = new ResultListModel
            {
                DatasetNames = dataContext.DeathClockDataSet.Select(d => d.Name).ToArray(),
                PersonList = dataContext.DeathClockDataSet[0].MostRisk()
            };
            return View(model);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }
    }
}
