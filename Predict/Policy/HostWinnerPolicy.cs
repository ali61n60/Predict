using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Predict.Infrastructure;
using Predict.Models;

namespace Predict.Policy
{
    public class HostWinnerPolicy:IPolicy
    {
        RankCalculator _rankCalculator;
        private int _winnerGoals;
        private int _loserGoals;
        private int _equalGoals;
        public HostWinnerPolicy(int winnerGoals,int loserGoals,int equalGoals,RankCalculator rankCalculator)
        {
            _rankCalculator = rankCalculator;
            _winnerGoals = winnerGoals;
            _loserGoals = loserGoals;
            _equalGoals = equalGoals;
            Name= $"HostWinner({_winnerGoals},{_loserGoals},{_equalGoals})";
        }

        public Prediction PredictMatch(Team hostTeam, Team guestTeam, int week)
        {
            Prediction myPrediction = null;
            int hostTeamRank = _rankCalculator.CalculateCurrentRank(week-1, hostTeam);
            int guestTeamRank = _rankCalculator.CalculateCurrentRank(week-1, guestTeam);

            if (hostTeamRank  < guestTeamRank) //host higher
            {
                myPrediction = new Prediction() { HostGoals = _winnerGoals, GuestGoals = _loserGoals };
            }
            else
            {
                myPrediction = new Prediction() { HostGoals = _equalGoals, GuestGoals = _equalGoals };
            }

            return myPrediction;
        }

        public string Name { get; }
    }
}
