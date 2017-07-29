using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Predict.Models;
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
            TeamRepository teamRepository=new TeamRepository();
            List<Team> allTeams = teamRepository.GetTeams();

            listBox1.Items.Clear();
            foreach (Team team in allTeams)
            {
                listBox1.Items.Add(team.Id + " : " + team.TeamName);
            }
        }
    }
}
