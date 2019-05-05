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
        private readonly DeathClockContext deathClockContext;

        public HomeController(DeathClockContext deathClockContext)
        {
            this.deathClockContext = deathClockContext ?? throw new ArgumentNullException(nameof(deathClockContext));
        }

        public IActionResult Index(string datasetName, int? deathYear, int page = 1)
        {
            IEnumerable<BasePerson> persons = null;
            switch(datasetName)
            {
                case Persistence.Constants.TMDB_DATASET_NAME:
                default:
                    persons = deathClockContext.TmdbPersons;
                    datasetName = Persistence.Constants.TMDB_DATASET_NAME;
                    break;

                case Persistence.Constants.WIKIPEDIA_DATASET_NAME:
                    persons = deathClockContext.WikipediaPersons.Where(p => !p.IsStub);
                    break;

                case Persistence.Constants.WIKIPEDIA_STUB_NAME:
                    persons = deathClockContext.WikipediaPersons.Where(p => p.IsStub);
                    break;
            }
            if (deathYear.HasValue)
            {
                persons = persons.ByDeathYear(deathYear.Value);
            }
            else
            {
                persons = persons.MostRisk();
            }

            var pagedPersonList = persons.Skip((page - 1) * Constants.ItemsPerPage)
                .Take(Constants.ItemsPerPage)
                .ToArray();

            var model = new ResultListModel
            {
                DatasetNames = new[] { Persistence.Constants.TMDB_DATASET_NAME, Persistence.Constants.WIKIPEDIA_DATASET_NAME, Persistence.Constants.WIKIPEDIA_STUB_NAME },
                DatasetName = datasetName,
                PersonList = pagedPersonList,
                DeathYear = deathYear,
                Page = page,
                TotalPages = (persons.Count() / Constants.ItemsPerPage) + 1
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
