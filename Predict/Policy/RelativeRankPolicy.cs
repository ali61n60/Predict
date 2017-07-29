using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Predict.Models;

namespace Predict.Policy
{
    public class RelativeRankPolicy:IPolicy
    {
        public Prediction PredictMatch(Team hostTeam, Team guestTeam, int week)
        {
            return new Prediction(){HostGoals = 1,GuestGoals = 1};
        }

        public string Name { get; }
    }
}
