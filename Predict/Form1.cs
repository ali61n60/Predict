﻿using System;
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


            //simplePolicyPredict(0, 0);
            //simplePolicyPredict(1, 1);
            //simplePolicyPredict(2, 2);
            //simplePolicyPredict(3, 3);
            //simplePolicyPredict(4, 4);
            RankCalculator rankCalculator = new RankCalculator();
            //for (int winnerRank = 0; winnerRank < 8; winnerRank++)
            //{
            //    for (int loserRank = 9; loserRank < 17; loserRank++)
            //    {
            //        for (int winnerGoals = 1; winnerGoals <= 5; winnerGoals++)
            //        {
            //            for (int loserGoals = 0; loserGoals < winnerGoals; loserGoals++)
            //            {
            //                for (int equalGoals = 0; equalGoals < 5; equalGoals++)
            //                {
            //                    dynamicPolicyPredict(winnerRank, loserRank, winnerGoals, loserGoals, equalGoals, rankCalculator);
            //                }
            //            }
            //        }

            //    }
            //}

            dynamicPolicyPredict(3, 13, 1, 0, 1, rankCalculator);
            listBox1.Items.Add("");
            listBox1.Items.Add("");
            listBox1.Items.Add("");
            listBox1.Items.Add("");
            dynamicPolicyPredict(3, 16, 1, 0, 1, rankCalculator);

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
            IPolicy policy = new HostWinnerPolicy(winnerGoals, loserGoals, equalGoals, rankCalculator);
            runPrediction(policy, _allMatches);
        }

        private void relativePolicyPredict(int winnerGoals, int loserGoals, int equalGoals, int relativeHighToWin, RankCalculator rankCalculator)
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
            Dictionary<int, int> totalScoreDictionary = new Dictionary<int, int>();


            foreach (MatchResult matchResult in allMatches)
            {
                currentPrediction = policy.PredictMatch(matchResult.HosTeam, matchResult.GuestTeam, matchResult.Week);
                int currentPredictionPoint = 0;

                if (currentPrediction.HostGoals == matchResult.HostGoals &&
                    currentPrediction.GuestGoals == matchResult.GuestGoals)
                {
                    currentPredictionPoint = 10;
                    exactPrediction++;
                }
                else if ((currentPrediction.HostGoals - currentPrediction.GuestGoals) ==
                         (matchResult.HostGoals - matchResult.GuestGoals))
                {
                    currentPredictionPoint = 7;
                    sameDiffPrediction++;
                }
                else if (Math.Sign(currentPrediction.HostGoals - currentPrediction.GuestGoals) ==
                         Math.Sign(matchResult.HostGoals - matchResult.GuestGoals))
                {
                    currentPredictionPoint = 5;
                    winnerOkPrediction++;
                }
                else
                {
                    currentPredictionPoint = 2;
                    wrongPrediction++;
                }
                addToDictionary(totalScoreDictionary, matchResult.Week, currentPredictionPoint);
                listBox1.Items.Add(string.Format("{0} ({1}) [{2}:{3}] ({4}) {5} ===>{6}", matchResult.HosTeam.TeamName,
                                                                                  currentPrediction.HostGoals,
                                                                                  matchResult.HostGoals,
                                                                                  matchResult.GuestGoals,
                                                                                  currentPrediction.GuestGoals,
                                                                                  matchResult.GuestTeam.TeamName,
                                                                                  currentPredictionPoint));
            }
            totalScore = exactPrediction * 10 + sameDiffPrediction * 7 + winnerOkPrediction * 5 + wrongPrediction * 2;
            totalMatches = exactPrediction + sameDiffPrediction + winnerOkPrediction + wrongPrediction;
            if (totalScore > 11)
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

        private void addToDictionary(Dictionary<int, int> totalScoreDictionary, int week, int score)
        {
            if (totalScoreDictionary.ContainsKey(week))
                totalScoreDictionary[week] += score;
            else
                totalScoreDictionary.Add(week, score);
        }
    }
}
