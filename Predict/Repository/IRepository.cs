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
    }

    public class TeamRepository : IRepository
    {
        readonly string _connectionString= System.Configuration.ConfigurationManager.
            ConnectionStrings["Predict.Properties.Settings.Foot820ConnectionString"].ConnectionString;

        public List<Team> GetTeams()
        {
            List<Team> allTeams=new List<Team>();
            Team tempTeam;
            string query = "SELECT * FROM Teams";
            SqlDataReader dataReader;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    //command.Parameters.Add("@start", SqlDbType.Int).Value = index;
                    //command.Parameters.Add("@end", SqlDbType.Int).Value = (index + count - 1);
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

        public string GetConnectionString()
        {
            return _connectionString;
        }
    }
}
