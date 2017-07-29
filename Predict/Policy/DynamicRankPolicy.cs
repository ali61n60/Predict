using System;
using Predict.Infrastructure;
using Predict.Models;

namespace Predict.Policy
{
    public class DynamicRankPolicy:IPolicy
    {
        private int _winnerRank;
        private int _loserRank;
        private int _winnerGoals;
        private int _loserGoals;
        private int _equalGoals;
        RankCalculator _rankCalculator;
        
        public DynamicRankPolicy(int winnerRank,int loserRank,int winnerGoals,int loserGoals,int equalGoals ,RankCalculator rankCalculator) 
        {
            _winnerRank = winnerRank;
            _loserRank = loserRank;
            _winnerGoals = winnerGoals;
            _loserGoals = loserGoals;
            _equalGoals = equalGoals;
            _rankCalculator = rankCalculator;
            
            Name =String.Format("DynamicPolicy({0},{1},{2},{3},{4}",_winnerRank,_loserRank,_winnerGoals,_loserGoals,_equalGoals);
        }
        public Prediction PredictMatch(Team hostTeam, Team guestTeam, int week)
        {
            Prediction myPrediction;
            int hostTeamRank = _rankCalculator.CalculateCurrentRank(week, hostTeam);
            int guestTeamRank = _rankCalculator.CalculateCurrentRank(week, guestTeam);

            if (hostTeamRank <= _winnerRank)//host winnerRank wins
            {
                myPrediction = new Prediction(){HostGoals = _winnerGoals,GuestGoals = _loserGoals};
            }
            else if (guestTeamRank <= _winnerRank)//guest winnerRank wins
            {
                myPrediction=new Prediction(){HostGoals = _loserGoals,GuestGoals = _winnerGoals};
            }
            else if(guestTeamRank>=_loserRank)//guest loserRank lose
            {
                myPrediction=new Prediction(){HostGoals = _winnerGoals,GuestGoals = _loserGoals};
            }
            else if(hostTeamRank>=_loserRank)//host loserRank lose
            {
                myPrediction = new Prediction() { HostGoals = _loserGoals, GuestGoals = _winnerGoals };
            }
            else
            {
                myPrediction=new Prediction(){HostGoals =_equalGoals,GuestGoals = _equalGoals};
            }
            return myPrediction;
        }

        public string Name { get; }
    }
}
