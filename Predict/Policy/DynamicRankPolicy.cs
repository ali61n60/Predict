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
        private int _hostTeamRank;
        private int _guestTeamRank;

        public string Name { get; }

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
            _hostTeamRank = _rankCalculator.CalculateCurrentRank(week, hostTeam);
            _guestTeamRank = _rankCalculator.CalculateCurrentRank(week, guestTeam);


            if (bothTeamsAreHighRank())
            {
                if (lastMatchGuestAndWon(hostTeam, week) || lastMatchHostAndLost(guestTeam,week) )//good host team won last game being gueset
                {
                    return hostWin();
                }
                else
                {
                    return equalResult();
                }
            }
            else if (hostTeamIsHighRank())
            {
                return hostWin();
            }
            else if (guestTeamIsHighRank())
            {
                return guestWin();
            }
            else if(bothTeamsAreLowRank())
            {
                if (lastMatchHostAndLost(guestTeam,week) || lastMatchGuestAndWon(hostTeam,week))
                {
                    return hostWin();
                }
                else
                {
                    return equalResult();
                }
            }
            else if (guestTeamIsLowRank())
            {
                if (lastMatchGuestAndWon(hostTeam, week) || lastMatchHostAndLost(guestTeam,week))
                {
                    return hostWin();
                }
                else if (last3MatchesScore(guestTeam, week) >= 4)
                {
                    return equalResult();
                }
                else
                {
                    return hostWin();
                }
            }
            else if (hostTeamIsLowRank())
            {
                return guestWin();
            }
            else
            {
                if (lastMatchGuestAndWon(hostTeam, week) ||lastMatchHostAndLost(guestTeam, week))
                {
                    return hostWin();
                }
                else
                {
                    return equalResult();
                }
            }
        }

        private Prediction guestWin()
        {
            return new Prediction() { HostGoals = _loserGoals, GuestGoals = _winnerGoals };
        }

        private Prediction equalResult()
        {
            return new Prediction() { HostGoals = _equalGoals, GuestGoals = _equalGoals };
        }

        private Prediction hostWin()
        {
            return new Prediction() { HostGoals = _winnerGoals, GuestGoals = _loserGoals };
        }

        private bool bothTeamsAreLowRank()
        {
            return hostTeamIsLowRank() && guestTeamIsLowRank();
        }

        private bool guestTeamIsLowRank()
        {
            return _guestTeamRank >= _loserRank;
        }

        private bool hostTeamIsLowRank()
        {
            return _hostTeamRank >= _loserRank;
        }

        private bool guestTeamIsHighRank()
        {

            return _guestTeamRank <= _winnerRank;
        }

        private bool hostTeamIsHighRank()
        {
            return _hostTeamRank <= _winnerRank;
        }

        private bool bothTeamsAreHighRank()
        {
            return hostTeamIsHighRank() && guestTeamIsHighRank();
        }

        private int last3MatchesScore(Team team, int currentWeek)
        {
            int last3matchscore = 0;
            if (currentWeek < 4)
                return 0;
            MatchResult lastWeekMatchResult = _allMatchResults.Find(result => result.Week == (currentWeek - 1) &&
                                                                              (result.HosTeam.Id == team.Id ||
                                                                               result.GuestTeam.Id == team.Id));
            last3matchscore += getMatchPoints(lastWeekMatchResult, team);
            MatchResult SecondWeekAgoMatchResult = _allMatchResults.Find(result => result.Week == (currentWeek - 2) &&
                                                                              (result.HosTeam.Id == team.Id ||
                                                                               result.GuestTeam.Id == team.Id));
            last3matchscore += getMatchPoints(SecondWeekAgoMatchResult, team);

            MatchResult ThirdWeekAgolastWeekMatchResult = _allMatchResults.Find(result => result.Week == (currentWeek - 3) &&
                                                                              (result.HosTeam.Id == team.Id ||
                                                                               result.GuestTeam.Id == team.Id));
            last3matchscore += getMatchPoints(ThirdWeekAgolastWeekMatchResult, team);
            return last3matchscore;
        }

        private int getMatchPoints(MatchResult matchResult, Team team)
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
    }
}
