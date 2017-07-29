using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Predict.Models;

namespace Predict.Policy
{
    interface IPolicy
    {
        Prediction PredictMatch(Team hostTeam, Team guestTeam, int week);
    }

    public class SimplePolicy : IPolicy
    {
        private int _hostGoals;
        private int _guestGoals;

        public SimplePolicy(int hostGoals, int guestGoals)
        {
            _hostGoals = hostGoals;
            _guestGoals = guestGoals;
        }
        public Prediction PredictMatch(Team hostTeam, Team guestTeam, int week)
        {
            return new Prediction()
            {
                HostGoals = _hostGoals,
                GuestGoals = _guestGoals
            };
        }
    }

}
