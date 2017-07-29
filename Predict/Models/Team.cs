using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Predict.Models
{
    public class Team
    {
        public int Id { get;private set; }
        public string TeamName { get; private set; }
        public int Points { get; set; }
        public int ScoredGoals { get; set; }
        public int RecievedGoals { get; set; }

        public Team(int id, string teamName)
        {
            Id = id;
            TeamName = teamName;
        }
    }
}
