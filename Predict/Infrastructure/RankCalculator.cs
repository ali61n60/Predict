using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Predict.Models;
using Predict.Repository;

namespace Predict.Infrastructure
{
    public class RankCalculator
    {
        public int CalculateCurrentRank(int week, Team team, List<MatchResult> matchResults)
        {
            IRepository teamRepository=new TeamRepository();
            List<Team> allTeams= teamRepository.GetTeams();
            Team tempHostTeam;
            Team tempGuestTeam;

            foreach (Team t in allTeams)
            {
                t.Points = 0;
                t.ScoredGoals = 0;
                t.RecievedGoals = 0;
                foreach (MatchResult matchResult in matchResults)
                {
                    if (matchResult.Week < week)
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
                        else if(matchResult.GuestTeam.Id == t.Id)//team is guest
                        {
                            t.ScoredGoals += matchResult.GuestGoals;
                            t.RecievedGoals += matchResult.HostGoals;
                            if (matchResult.GuestGoals> matchResult.HostGoals)
                                t.Points += 3;
                            else if (matchResult.GuestGoals==matchResult.HostGoals )
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
            return allTeams.FindIndex(t => t.Id == team.Id)+1;
        }
    }
}
