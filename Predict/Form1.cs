using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Predict.Infrastructure;
using Predict.Models;
using Predict.Policy;
using Predict.Repository;

namespace Predict
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            textBox1.Text=new TeamRepository().GetConnectionString();
        }

        private void buttonGetAllTeams_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
           

            predict(0,0);
            predict(1, 1);
            predict(2, 2);
            predict(3, 3);
            predict(4, 4);
            predict(5, 5);
            predict(6, 6);
            predict(7, 7);
            predict(8, 8);
            
        }

        private void predict(int a,int b)
        {
            IPolicy policy=new SimplePolicy(a,b);
            IRepository repository=new TeamRepository();
            List<MatchResult> allMatches= repository.GetMatchResults();
            Prediction currentPrediction;

            int exactPrediction=0;
            int sameDiffPrediction=0;
            int winnerOkPrediction=0;
            int wrongPrediction=0;
            int totalScore=0;
            int totalMatches = 0;
            
            foreach (MatchResult matchResult in allMatches)
            {
                currentPrediction = policy.PredictMatch(matchResult.HosTeam, matchResult.GuestTeam, matchResult.Week);
                if (currentPrediction.HostGoals == matchResult.HostGoals && currentPrediction.GuestGoals == matchResult.GuestGoals)
                    exactPrediction++;
                else if((currentPrediction.HostGoals-currentPrediction.GuestGoals)==(matchResult.HostGoals-matchResult.GuestGoals))
                   sameDiffPrediction++;
                else if (Math.Sign(currentPrediction.HostGoals - currentPrediction.GuestGoals) ==
                         Math.Sign(matchResult.HostGoals - matchResult.GuestGoals))
                    winnerOkPrediction++;
                else
                    wrongPrediction++;
                

            }
            totalScore = exactPrediction * 10 + sameDiffPrediction * 7 + winnerOkPrediction * 5 + wrongPrediction * 2;
            totalMatches= exactPrediction + sameDiffPrediction + winnerOkPrediction + wrongPrediction;
            listBox1.Items.Add("for ("+a+","+b  +") ===>>> Exact: " + exactPrediction+
                               " , SameDiff: "+sameDiffPrediction +
                               " , WinnerOk: "+winnerOkPrediction+
                               " , WrongGuess: "+wrongPrediction+
                               " ,TotalMatches: "+totalMatches+
                               " , Total " + totalScore);
        }
    }
}
