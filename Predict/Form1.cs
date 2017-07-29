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
            textBox1.Text = new TeamRepository().GetConnectionString();
            _allMatches = _repository.GetMatchResults();
        }

        private int maxScore = 0;
        private IRepository _repository = new TeamRepository();
        private List<MatchResult> _allMatches;

        private void buttonGetAllTeams_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();


            simplePolicyPredict(0, 0);
            simplePolicyPredict(1, 1);
            simplePolicyPredict(2, 2);
            simplePolicyPredict(3, 3);
            simplePolicyPredict(4, 4);
            RankCalculator rankCalculator = new RankCalculator();
            for (int winnerRand = 0; winnerRand < 5; winnerRand++)
            {
                for (int loserRank = 11; loserRank < 17; loserRank++)
                {
                    for (int winnerGoals = 1; winnerGoals <= 5; winnerGoals++)
                    {
                        for (int loserGoals = 0; loserGoals < winnerGoals; loserGoals++)
                        {
                            for (int equalGoals = 0; equalGoals < 5; equalGoals++)
                            {
                                dynamicPolicyPredict(winnerRand, loserRank, winnerGoals, loserGoals, equalGoals, rankCalculator);
                            }
                        }
                    }

                }
            }


            //for (int winnerGoals = 1; winnerGoals <= 5; winnerGoals++)
            //{
            //    for (int loserGoals = 0; loserGoals < winnerGoals; loserGoals++)
            //    {
            //        for (int equalGoals = 0; equalGoals < 5; equalGoals++)
            //        {
            //            for (int relativeHighToWin = 1; relativeHighToWin < 10; relativeHighToWin++)
            //            {
            //                relativePolicyPredict(winnerGoals, loserGoals, equalGoals, relativeHighToWin, rankCalculator);
            //            }
            //        }

            //    }
            //}

            //for (int winnerGoals = 1; winnerGoals <= 5; winnerGoals++)
            //{
            //    for (int loserGoals = 0; loserGoals < winnerGoals; loserGoals++)
            //    {
            //        for (int equalGoals = 0; equalGoals < 5; equalGoals++)
            //        {
            //            HostWinPolicyPredict(winnerGoals, loserGoals, equalGoals, rankCalculator);
            //        }
            //    }
            //}




            listBox1.Items.Add("MaxScore is : " + maxScore);
        }

        private void HostWinPolicyPredict(int winnerGoals, int loserGoals, int equalGoals, RankCalculator rankCalculator)
        {
            IPolicy policy = new HostWinnerPolicy(winnerGoals, loserGoals, equalGoals,rankCalculator);
            runPrediction(policy, _allMatches);
        }

        private void relativePolicyPredict(int winnerGoals, int loserGoals, int equalGoals, int relativeHighToWin,RankCalculator rankCalculator)
        {
            IPolicy policy = new RelativeRankPolicy(winnerGoals, loserGoals, equalGoals, relativeHighToWin,
                rankCalculator);
            runPrediction(policy, _allMatches);
        }

        private void dynamicPolicyPredict(int winnerRank, int loserRank, int winnerGolas, int loserGoals, int equalGoals, RankCalculator rankCalculator)
        {
            IPolicy policy = new DynamicRankPolicy(winnerRank, loserRank, winnerGolas, loserGoals, equalGoals, rankCalculator, _allMatches);
            runPrediction(policy, _allMatches);
        }

        private void simplePolicyPredict(int hostGoals, int guestGoals)
        {
            IPolicy policy = new SimplePolicy(hostGoals, guestGoals);
            runPrediction(policy, _allMatches);
        }

        private void runPrediction(IPolicy policy, List<MatchResult> allMatches)
        {
            Prediction currentPrediction;
            int exactPrediction = 0;
            int sameDiffPrediction = 0;
            int winnerOkPrediction = 0;
            int wrongPrediction = 0;
            int totalScore = 0;
            int totalMatches = 0;

            foreach (MatchResult matchResult in allMatches)
            {
                currentPrediction = policy.PredictMatch(matchResult.HosTeam, matchResult.GuestTeam, matchResult.Week);
                if (currentPrediction.HostGoals == matchResult.HostGoals && currentPrediction.GuestGoals == matchResult.GuestGoals)
                    exactPrediction++;
                else if ((currentPrediction.HostGoals - currentPrediction.GuestGoals) == (matchResult.HostGoals - matchResult.GuestGoals))
                    sameDiffPrediction++;
                else if (Math.Sign(currentPrediction.HostGoals - currentPrediction.GuestGoals) ==
                         Math.Sign(matchResult.HostGoals - matchResult.GuestGoals))
                    winnerOkPrediction++;
                else
                    wrongPrediction++;


            }
            totalScore = exactPrediction * 10 + sameDiffPrediction * 7 + winnerOkPrediction * 5 + wrongPrediction * 2;
            totalMatches = exactPrediction + sameDiffPrediction + winnerOkPrediction + wrongPrediction;
            if (totalScore > 1200)
            {
                listBox1.Items.Add(policy.Name + " ===>>> Exact: " + exactPrediction +
                                   " , SameDiff: " + sameDiffPrediction +
                                   " , WinnerOk: " + winnerOkPrediction +
                                   " , WrongGuess: " + wrongPrediction +
                                   " ,TotalMatches: " + totalMatches +
                                   " , Total " + totalScore);
            }
            if (totalScore > maxScore)
                maxScore = totalScore;
        }
    }
}
