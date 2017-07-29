﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Predict.Models;
using Predict.Repository;

namespace Predict.Infrastructure
{
    public class RankWeek
    {
        public int teamId;
        public int rank;
        public int week;
    }
    public class RankCalculator
    {
        //TODO improve CalculateCurrentRank algorithm to be faster
        private bool alreadyCalculated = false;
        private List<RankWeek> rankWeeks = new List<RankWeek>();
        private List<Team> allTeams;
        private List<MatchResult> matchResults;
        public int CalculateCurrentRank(int week, Team team)
        {
            if (alreadyCalculated)
            {
                return rankWeeks.Find(rankWeek => (rankWeek.week == week && rankWeek.teamId == team.Id)).rank;
            }
            else
            {
                calculateAllWeeksRanks();
                alreadyCalculated = true;
                return CalculateCurrentRank(week, team);
            }

            //return allTeams.FindIndex(t => t.Id == team.Id)+1;
        }

        private void calculateAllWeeksRanks()
        {
            IRepository teamRepository = new TeamRepository();
            allTeams = teamRepository.GetTeams();
            matchResults = teamRepository.GetMatchResults();
            int numberOfWeeks = matchResults.Count * 2 / allTeams.Count;
            rankWeeks.Clear();
            for (int week = 1; week <= numberOfWeeks; week++)
            {
                rankWeeks.AddRange(calculateRanksForWeek(week));
            }
        }

        private List<RankWeek> calculateRanksForWeek(int week)
        {
            Team tempHostTeam;
            Team tempGuestTeam;
            List<RankWeek> cuurentWeekRanks=new List<RankWeek>();
            foreach (Team t in allTeams)
            {
                t.Points = 0;
                t.ScoredGoals = 0;
                t.RecievedGoals = 0;
                foreach (MatchResult matchResult in matchResults)
                {
                    if (matchResult.Week <= week)
                    {
                        if (matchResult.HosTeam.Id == t.Id)//team is host
                        {
                            t.ScoredGoals += matchResult.HostGoals;
                            t.RecievedGoals += matchResult.GuestGoals;
                            if (matchResult.HostGoals > matchResult.GuestGoals)
                                t.Points += 3;
                            else if (matchResult.HostGoals == matchResult.GuestGoals)
                                t.Points += 1;

                        }
                        else if (matchResult.GuestTeam.Id == t.Id)//team is guest
                        {
                            t.ScoredGoals += matchResult.GuestGoals;
                            t.RecievedGoals += matchResult.HostGoals;
                            if (matchResult.GuestGoals > matchResult.HostGoals)
                                t.Points += 3;
                            else if (matchResult.GuestGoals == matchResult.HostGoals)
                                t.Points += 1;
                        }
                    }
                }
            }
            allTeams.Sort((b, a) =>
                {
                    if (a.Points > b.Points)
                        return 1;
                    else if (a.Points < b.Points)
                        return -1;
                    else
                    {
                        if ((a.ScoredGoals - a.RecievedGoals) > (b.ScoredGoals - b.RecievedGoals)) // tafazol Good
                            return 1;
                        else if ((a.ScoredGoals - a.RecievedGoals) < (b.ScoredGoals - b.RecievedGoals)) // tafazol Bad
                            return -1;
                        else
                        {
                            if (a.ScoredGoals > b.ScoredGoals)//gol zadeh Good
                                return 1;
                            else if (a.ScoredGoals < b.ScoredGoals) //gol zadeh BAd
                                return -1;
                        }
                    }
                    return 0;
                }
            );
            for(int i=0;i<allTeams.Count;i++)
            {
                RankWeek tempWeek=new RankWeek(){rank = i+1,teamId = allTeams[i].Id,week = week};
                cuurentWeekRanks.Add(tempWeek);
            }
            return cuurentWeekRanks;
        }
    }
}
