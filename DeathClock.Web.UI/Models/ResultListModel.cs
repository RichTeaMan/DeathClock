using DeathClock.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeathClock.Web.UI.Models
{
    public class ResultListModel
    {
        public string[] DatasetNames { get; set; }

        public Person[] PersonList { get; set; }
    }
}
