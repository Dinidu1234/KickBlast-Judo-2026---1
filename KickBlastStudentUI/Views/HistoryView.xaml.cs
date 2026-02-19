using System.Windows.Controls;
using KickBlastStudentUI.Data;
using KickBlastStudentUI.Helpers;
using KickBlastStudentUI.Models;

namespace KickBlastStudentUI.Views;

public partial class HistoryView : UserControl
{
    private readonly Action<string> _status;

    public HistoryView(Action<string> status)
    {
        InitializeComponent();
        _status = status;
        LoadFilters();
        LoadHistory();
    }

    private void LoadFilters()
    {
        AthleteFilterComboBox.Items.Add("All");
        foreach (var athlete in Db.GetAthletes()) AthleteFilterComboBox.Items.Add(athlete.Name);
        AthleteFilterComboBox.SelectedIndex = 0;

        MonthComboBox.Items.Add("All");
        for (var m = 1; m <= 12; m++) MonthComboBox.Items.Add(m.ToString());
        MonthComboBox.SelectedIndex = 0;

        YearComboBox.Items.Add("All");
        for (var y = DateTime.Now.Year - 2; y <= DateTime.Now.Year + 1; y++) YearComboBox.Items.Add(y.ToString());
        YearComboBox.SelectedItem = DateTime.Now.Year.ToString();
    }

    private void LoadHistory()
    {
        var athlete = AthleteFilterComboBox.SelectedItem?.ToString() ?? "All";
        var month = MonthComboBox.SelectedItem?.ToString() == "All" ? 0 : int.Parse(MonthComboBox.SelectedItem?.ToString() ?? "0");
        var year = YearComboBox.SelectedItem?.ToString() == "All" ? 0 : int.Parse(YearComboBox.SelectedItem?.ToString() ?? "0");

        HistoryGrid.ItemsSource = Db.GetHistory(athlete, month, year);
        _status("History loaded.");
    }

    private void Apply_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        LoadHistory();
    }

    private void HistoryGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (HistoryGrid.SelectedItem is MonthlyCalculation row)
        {
            DetailsText.Text = $"Date: {row.Date}\nAthlete: {row.AthleteName}\nPlan: {row.Plan}\n" +
                               $"Training: {CurrencyHelper.ToLkr(row.TrainingCost)}\n" +
                               $"Coaching: {CurrencyHelper.ToLkr(row.CoachingCost)}\n" +
                               $"Competition: {CurrencyHelper.ToLkr(row.CompetitionCost)}\n" +
                               $"Total: {CurrencyHelper.ToLkr(row.TotalCost)}\n" +
                               $"Weight: {row.WeightMessage}\nSecond Saturday: {row.SecondSaturday}";
        }
    }
}
