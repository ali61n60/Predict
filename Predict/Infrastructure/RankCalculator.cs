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
        public int CalculateCurrentRank(int week, int teamId, List<MatchResult> matchResults)
        {
            IRepository teamRepository=new TeamRepository();
            List<Team> allTeams= teamRepository.GetTeams();
            Team tempHostTeam;
            Team tempGuestTeam;

            foreach (Team team in allTeams)
            {
                team.Points = 0;
                team.ScoredGoals = 0;
                team.RecievedGoals = 0;
                foreach (MatchResult matchResult in matchResults)
                {
                    if (matchResult.Week < week && (matchResult.HosTeam.Id == team.Id ||
                                                    matchResult.GuestTeam.Id == team.Id))
                    {
                        tempHostTeam = matchResult.HosTeam;
                        tempGuestTeam = matchResult.GuestTeam;

                    }
                }
            }
            return 1;
        }
    }
}
