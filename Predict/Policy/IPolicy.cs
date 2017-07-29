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
        string Name { get; }
    }
}
