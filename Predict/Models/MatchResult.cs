using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Predict.Models
{
    public class MatchResult
    {
        public Team HosTeam { get; private set; }
        public Team GuestTeam { get; set; }
        public int HostGoals { get; private set; }
        public int GuestGoals { get; private set; }
        public int Week { get; private set; }

        public MatchResult(Team hosTeam, int hostGoals, Team guestTeam, int guestGoals, int week)
        {
            HosTeam = hosTeam;
            GuestTeam = guestTeam;
            HostGoals = hostGoals;
            GuestGoals = guestGoals;
            Week = week;
        }
    }
}
