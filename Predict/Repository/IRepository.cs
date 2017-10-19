using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Predict.Models;

namespace Predict.Repository
{
    public interface IRepository
    {
        List<Team> GetTeams();
        List<MatchResult> GetMatchResults();
    }

    public class TeamRepository : IRepository
    {
        readonly string _connectionString= System.Configuration.ConfigurationManager.
            ConnectionStrings["Predict.Properties.Settings.Foot820ConnectionString"].ConnectionString;

        public List<Team> GetTeams()
        {
            List<Team> allTeams=new List<Team>();
            Team tempTeam;
            string query = "SELECT Teams.Id,Teams.TeamName "+
                " FROM Teams INNER JOIN TeamsInYears ON Teams.Id = TeamsInYears.TeamId "+
                " WHERE TeamsInYears.Year = 1396 ";
            SqlDataReader dataReader;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    dataReader= command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        tempTeam=new Team((int)dataReader["Id"],dataReader["TeamName"].ToString());
                        allTeams.Add(tempTeam);
                    }
                }
            }
            return allTeams;
        }

        public List<MatchResult> GetMatchResults()
        {
            List<MatchResult> allMatchResults=new List<MatchResult>();
            List<Team> allTeams = GetTeams();
            MatchResult tempMatchResult;
            string query = " SELECT Matchs.Id,Matchs.Week, H.TeamName AS Host,Matchs.HostGoals,Matchs.GuestGoals, " +
                           " G.TeamName AS Guest FROM Matchs INNER JOIN Teams AS H ON Matchs.HostId = H.Id " +
                           " INNER JOIN Teams AS G ON Matchs.GuestId = G.Id "+
                           " WHERE Matchs.Year=1396 ";
            SqlDataReader dataReader;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        tempMatchResult = new MatchResult(
                            allTeams.Find(team => team.TeamName == dataReader["Host"].ToString()),
                            (int) dataReader["HostGoals"],
                            allTeams.Find(team => team.TeamName == dataReader["Guest"].ToString()),
                            (int) dataReader["GuestGoals"],
                            (int) dataReader["Week"]);
                        allMatchResults.Add(tempMatchResult);
                    }
                }
            }
            return allMatchResults;

        }

        public string GetConnectionString()
        {
            return _connectionString;
        }
    }
}
