using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DeathClock.Web.UI.Models;
using DeathClock.Persistence;

namespace DeathClock.Web.UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataContext dataContext;

        public HomeController(DataContext dataContext)
        {
            this.dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
        }

        public IActionResult Index(string datasetName, int? deathYear)
        {
            var dataset = dataContext.DeathClockDataSet.FirstOrDefault(d => d.Name == datasetName);
            if (dataset == null)
            {
                dataset = dataContext.DeathClockDataSet.First();
            }
            Person[] personList;
            if (deathYear.HasValue)
            {
                personList = dataset.ByDeathYear(deathYear.Value);
            }
            else
            {
                personList = dataset.MostRisk();
            }
            var model = new ResultListModel
            {
                DatasetNames = dataContext.DeathClockDataSet.Select(d => d.Name).ToArray(),
                DatasetName = dataset.Name,
                PersonList = personList,
                DeathYear = deathYear
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
