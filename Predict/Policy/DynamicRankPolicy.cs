using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Predict.Infrastructure;
using Predict.Models;
using Predict.Repository;

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
        IRepository _teamRepository=new TeamRepository();
        private List<MatchResult> _allMatchResults;
        

        public DynamicRankPolicy(int winnerRank,int loserRank,int winnerGoals,int loserGoals,int equalGoals ,RankCalculator rankCalculator) 
        {
            _winnerRank = winnerRank;
            _loserRank = loserRank;
            _winnerGoals = winnerGoals;
            _loserGoals = loserGoals;
            _equalGoals = equalGoals;
            _rankCalculator = rankCalculator;
            _allMatchResults = _teamRepository.GetMatchResults();
            Name = "DynamicPolicy("+
                _winnerRank+","+_loserRank+" , "+
                _winnerGoals+" , "+_loserGoals+" , "+_equalGoals+" )";
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
