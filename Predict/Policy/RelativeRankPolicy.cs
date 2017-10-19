using Predict.Infrastructure;
using Predict.Models;

namespace Predict.Policy
{
    public class RelativeRankPolicy:IPolicy
    {
        readonly RankCalculator _rankCalculator;
        private readonly int _equalGoals;
        private readonly int _winnerGoals;
        private readonly int _loserGoals;
        private readonly int _relativeHighToWin;
        public RelativeRankPolicy(int winnerGoals,int loserGoals,int equalGoals,int relativeHighToWin, RankCalculator rankCalculator)
        {
            _winnerGoals = winnerGoals;
            _loserGoals = loserGoals;
            _equalGoals = equalGoals;
            _relativeHighToWin = relativeHighToWin;
            _rankCalculator = rankCalculator;
            Name = $"RelativePolicy({_winnerGoals},{_loserGoals},{_equalGoals},{_relativeHighToWin})";
        }
        public Prediction PredictMatch(Team hostTeam, Team guestTeam, int week)
        {
            Prediction myPrediction=null;
            int hostTeamRank = _rankCalculator.CalculateCurrentRank(week-1, hostTeam);
            int guestTeamRank = _rankCalculator.CalculateCurrentRank(week-1, guestTeam);

            if ((hostTeamRank+_relativeHighToWin) < guestTeamRank) //host higher
            {
                myPrediction=new Prediction(){HostGoals = _winnerGoals,GuestGoals = _loserGoals};
            }
            //else if ((guestTeamRank + _relativeHighToWin)< hostTeamRank )//guest higher
            //{
            //    myPrediction=new Prediction(){GuestGoals = _winnerGoals,HostGoals = _loserGoals};
            //}
            else
            {
                myPrediction=new Prediction(){HostGoals = _equalGoals,GuestGoals = _equalGoals};
            }

            return myPrediction;
        }

        public string Name { get; }
    }
}
