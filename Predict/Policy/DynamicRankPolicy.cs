using System;
using System.Collections.Generic;
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
        private List<MatchResult> _allMatchResults;
        
        public DynamicRankPolicy(int winnerRank,int loserRank,
            int winnerGoals,int loserGoals,int equalGoals ,RankCalculator rankCalculator,List<MatchResult> allMatchResults) 
        {
            _winnerRank = winnerRank;
            _loserRank = loserRank;
            _winnerGoals = winnerGoals;
            _loserGoals = loserGoals;
            _equalGoals = equalGoals;
            _rankCalculator = rankCalculator;
            _allMatchResults = allMatchResults;
            Name = $"DynamicPolicy({_winnerRank},{_loserRank},{_winnerGoals},{_loserGoals},{_equalGoals})";
        }
        public Prediction PredictMatch(Team hostTeam, Team guestTeam, int week)
        {
            Prediction myPrediction;
            int hostTeamRank = _rankCalculator.CalculateCurrentRank(week, hostTeam);
            int guestTeamRank = _rankCalculator.CalculateCurrentRank(week, guestTeam);

            if (hostTeamRank <= _winnerRank && guestTeamRank <= _winnerGoals)//both good
            {
                if (lastMatchGuestAndWon(hostTeam, week))//good host team won last game being gueset
                {
                    myPrediction=new Prediction(){HostGoals = _winnerGoals,GuestGoals = _loserGoals};
                }
                else
                {
                    myPrediction = new Prediction() { HostGoals = _equalGoals, GuestGoals = _equalGoals };
                }
            }
            else if (hostTeamRank <= _winnerRank)//host winnerRank wins
            {
                myPrediction = new Prediction() { HostGoals = _winnerGoals, GuestGoals = _loserGoals };
            }
            else if (guestTeamRank <= _winnerRank)//guest winnerRank wins
            {
                myPrediction = new Prediction() { HostGoals = _loserGoals, GuestGoals = _winnerGoals };
            }
            else if(guestTeamRank >= _loserRank && hostTeamRank >= _loserRank)//both bad
            {
                if (lastMatchHostAndLost(guestTeam,week))//bad guest team lost last game being host
                {
                    myPrediction=new Prediction(){HostGoals = _winnerGoals,GuestGoals = _loserGoals};
                }
                else
                {
                    myPrediction = new Prediction() { HostGoals = _equalGoals, GuestGoals = _equalGoals };
                }
            }
            else if (guestTeamRank >= _loserRank)//guest loserRank lose
            {
                myPrediction = new Prediction() { HostGoals = _winnerGoals, GuestGoals = _loserGoals };
            }
            else if (hostTeamRank >= _loserRank)//host loserRank lose
            {
                myPrediction = new Prediction() { HostGoals = _loserGoals, GuestGoals = _winnerGoals };
            }
            else
            {
                if (lastMatchGuestAndWon(hostTeam, week) || lastMatchHostAndLost(guestTeam, week))
                {
                    myPrediction = new Prediction() {HostGoals = _winnerGoals, GuestGoals = _loserGoals};
                }
                else
                {
                    myPrediction = new Prediction() { HostGoals = _equalGoals, GuestGoals = _equalGoals };
                }
            }
            return myPrediction;
        }

        

        private bool lastMatchHostAndLost(Team guestTeam,int week)
        {
            if (week == 1)
                return false;
            MatchResult lastMatchResult =
                _allMatchResults.Find(result => result.Week == (week - 1) && result.HosTeam.Id == guestTeam.Id);
            if (lastMatchResult != null && lastMatchResult.HostGoals < lastMatchResult.GuestGoals)
                return true;
            return false;
        }

        private bool lastMatchGuestAndWon(Team hostTeam, int week)
        {
            if (week == 1)
                return false;
            MatchResult lastMatchResult= _allMatchResults.Find(result => result.Week == (week - 1) && result.GuestTeam.Id == hostTeam.Id);
            if (lastMatchResult != null && lastMatchResult.GuestGoals > lastMatchResult.HostGoals)
                return true;
            return false;
        }

        public string Name { get; }
    }
}
