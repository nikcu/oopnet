using System.Windows;
using System.Windows.Input;
using DataLayer.Models;

namespace WpfApp.Windows
{
    public partial class TeamInfoWindow : Window
    {
        private readonly TeamResult _team;

        public TeamInfoWindow(TeamResult team)
        {
            InitializeComponent();
            _team = team;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Display team info
            textTeamName.Text = _team.Country;
            textFifaCode.Text = $"({_team.FifaCode})";

            // Display statistics
            textMatchesPlayed.Text = _team.GamesPlayed.ToString();
            textWins.Text = _team.Wins.ToString();
            textDraws.Text = _team.Draws.ToString();
            textLosses.Text = _team.Losses.ToString();
            textGoalsFor.Text = _team.GoalsFor.ToString();
            textGoalsAgainst.Text = _team.GoalsAgainst.ToString();
            textGoalDifference.Text = _team.GoalDifferential.ToString();
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }
    }
}
