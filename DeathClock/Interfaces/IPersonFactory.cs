using System.Collections.Generic;
using System.Threading.Tasks;
using DeathClock.Persistence;

namespace DeathClock.Tmdb
{
    public interface IPersonFactory<TPerson> where TPerson : BasePerson
    {
        Task UpdateExistingPersons();

        Task FindNewPersons();
    }
}