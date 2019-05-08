using System.Threading.Tasks;
using DeathClock.Persistence;

namespace DeathClock
{
    public interface IPersonFactory<TPerson> where TPerson : BasePerson
    {
        Task UpdateExistingPersons();

        Task FindNewPersons();
    }
}