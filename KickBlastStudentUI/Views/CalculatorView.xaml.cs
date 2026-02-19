using System.Windows;
using System.Windows.Controls;
using KickBlastStudentUI.Data;
using KickBlastStudentUI.Helpers;
using KickBlastStudentUI.Models;

namespace KickBlastStudentUI.Views;

public partial class CalculatorView : UserControl
{
    private readonly Action<string> _status;
    private MonthlyCalculation? _last;

    public CalculatorView(Action<string> status)
    {
        InitializeComponent();
        _status = status;
        AthleteComboBox.ItemsSource = Db.GetAthletes();
        if (AthleteComboBox.Items.Count > 0) AthleteComboBox.SelectedIndex = 0;
    }

    private void Calculate_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (AthleteComboBox.SelectedItem is not Athlete athlete)
            {
                _status("Select an athlete.");
                return;
            }

            if (!ValidationHelper.TryParsePositiveInt(CompetitionsTextBox.Text, out var competitions))
            {
                _status("Competitions must be a positive integer.");
                return;
            }

            if (!ValidationHelper.TryParseDoubleInRange(CoachingHoursTextBox.Text, 0, 5, out var hours))
            {
                _status("Coaching hours must be 0 to 5.");
                return;
            }

            var pricing = Db.GetPricing();
            if (athlete.Plan == "Beginner") competitions = 0;

            var weeklyFee = athlete.Plan switch
            {
                "Intermediate" => pricing.IntermediateFee,
                "Elite" => pricing.EliteFee,
                _ => pricing.BeginnerFee
            };

            var training = weeklyFee * 4;
            var coaching = hours * 4 * pricing.CoachingRate;
            var competitionCost = competitions * pricing.CompetitionFee;
            var total = training + coaching + competitionCost;

            var message = athlete.CurrentWeight > athlete.CategoryWeight
                ? "Over target"
                : athlete.CurrentWeight < athlete.CategoryWeight
                    ? "Under target"
                    : "On target";

            var secondSaturday = DateHelper.GetSecondSaturday(DateTime.Now).ToString("yyyy-MM-dd");

            _last = new MonthlyCalculation
            {
                Date = DateTime.Now.ToString("yyyy-MM-dd"),
                AthleteName = athlete.Name,
                Plan = athlete.Plan,
                Competitions = competitions,
                CoachingHours = hours,
                TrainingCost = training,
                CoachingCost = coaching,
                CompetitionCost = competitionCost,
                TotalCost = total,
                WeightMessage = message,
                SecondSaturday = secondSaturday
            };

            ResultText.Text = $"Athlete: {athlete.Name}\nPlan: {athlete.Plan}\n\n" +
                              $"Training:    {CurrencyHelper.ToLkr(training)}\n" +
                              $"Coaching:    {CurrencyHelper.ToLkr(coaching)}\n" +
                              $"Competition: {CurrencyHelper.ToLkr(competitionCost)}\n" +
                              $"------------------------------\n" +
                              $"Total:       {CurrencyHelper.ToLkr(total)}\n\n" +
                              $"Weight Status: {message}\n" +
                              $"Second Saturday: {secondSaturday}";
            _status("Calculation complete.");
        }
        catch
        {
            _status("Calculation failed.");
        }
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        if (_last == null)
        {
            _status("Calculate first.");
            return;
        }

        Db.SaveCalculation(_last);
        _status("Calculation saved.");
    }
}
