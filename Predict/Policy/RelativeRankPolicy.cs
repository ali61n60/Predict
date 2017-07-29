using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Predict.Infrastructure;
using Predict.Models;

namespace Predict.Policy
{
    public class RelativeRankPolicy:IPolicy
    {
        RankCalculator _rankCalculator;
        private int _equalGoals;
        private int _winnerGoals;
        private int _loserGoals;
        private int _relativeHighToWin;
        public RelativeRankPolicy(int winnerGoals,int loserGoals,int equalGoals,int relativeHighToWin, RankCalculator rankCalculator)
        {
            _winnerGoals = winnerGoals;
            _loserGoals = loserGoals;
            _equalGoals = equalGoals;
            _relativeHighToWin = relativeHighToWin;
            _rankCalculator = rankCalculator;
            Name = String.Format("RelativePolicy({0},{1},{2},{3}",_winnerGoals,_loserGoals,_equalGoals,_relativeHighToWin);
        }
        public Prediction PredictMatch(Team hostTeam, Team guestTeam, int week)
        {
            Prediction myPrediction=null;
            int hostTeamRank = _rankCalculator.CalculateCurrentRank(week, hostTeam);
            int guestTeamRank = _rankCalculator.CalculateCurrentRank(week, guestTeam);

            if ((hostTeamRank+_relativeHighToWin) < guestTeamRank) //host higher
            {
                myPrediction=new Prediction(){HostGoals = _winnerGoals,GuestGoals = _loserGoals};
            }
            else if ((guestTeamRank + _relativeHighToWin)< hostTeamRank )//guest higher
            {
                myPrediction=new Prediction(){GuestGoals = _winnerGoals,HostGoals = _loserGoals};
            }
            else
            {
                myPrediction=new Prediction(){HostGoals = _equalGoals,GuestGoals = _equalGoals};
            }

            return myPrediction;
        }

        public string Name { get; }
    }
}
