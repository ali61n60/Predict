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
            //TODO add 94,93 league data and validate the model
            //TODO use neural-network model if you want
            Prediction myPrediction;
            int hostTeamRank = _rankCalculator.CalculateCurrentRank(week, hostTeam);
            int guestTeamRank = _rankCalculator.CalculateCurrentRank(week, guestTeam);

            if (hostTeamRank <= _winnerRank && guestTeamRank <= _winnerRank)//both good
            {
                if (lastMatchGuestAndWon(hostTeam, week) || lastMatchHostAndLost(guestTeam,week) )//good host team won last game being gueset
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
                myPrediction = new Prediction() {HostGoals = _loserGoals, GuestGoals = _winnerGoals};
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
                if (last3MatchesScore(guestTeam, week) >= 4)//guest team scored 4 point in last 3 games maxScore 1287
                {
                    myPrediction = new Prediction() { HostGoals = _equalGoals, GuestGoals = _equalGoals };
                }
                else
                {
                    myPrediction = new Prediction() { HostGoals = _winnerGoals, GuestGoals = _loserGoals };
                }
            }
            else if (hostTeamRank >= _loserRank)//host loserRank lose
            {
                 myPrediction = new Prediction() {HostGoals = _loserGoals, GuestGoals = _winnerGoals};
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

        private int last3MatchesScore(Team team, int currentWeek)
        {
            int last3matchscore = 0;
            if (currentWeek < 4)
                return 0;
            MatchResult lastWeekMatchResult = _allMatchResults.Find(result => result.Week == (currentWeek - 1) &&
                                                                              (result.HosTeam.Id == team.Id ||
                                                                               result.GuestTeam.Id == team.Id));
            last3matchscore += getMatchScore(lastWeekMatchResult, team);
            MatchResult SecondWeekAgoMatchResult = _allMatchResults.Find(result => result.Week == (currentWeek - 2) &&
                                                                              (result.HosTeam.Id == team.Id ||
                                                                               result.GuestTeam.Id == team.Id));
            last3matchscore += getMatchScore(SecondWeekAgoMatchResult, team);

            MatchResult ThirdWeekAgolastWeekMatchResult = _allMatchResults.Find(result => result.Week == (currentWeek - 3) &&
                                                                              (result.HosTeam.Id == team.Id ||
                                                                               result.GuestTeam.Id == team.Id));
            last3matchscore += getMatchScore(ThirdWeekAgolastWeekMatchResult, team);
            return last3matchscore;
        }

        private int getMatchScore(MatchResult matchResult, Team team)
        {
            if (matchResult.HosTeam.Id == team.Id) //host
            {
                if (matchResult.HostGoals > matchResult.GuestGoals)
                    return 3;
                else if(matchResult.HostGoals == matchResult.GuestGoals)
                {
                    return 1;
                }
            }
            else//guest
            {
                if (matchResult.GuestGoals > matchResult.HostGoals)
                    return 3;
                else if (matchResult.HostGoals == matchResult.GuestGoals)
                {
                    return 1;
                }
            }
            return 0;
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
