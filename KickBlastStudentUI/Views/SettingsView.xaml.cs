using System.Windows;
using System.Windows.Controls;
using KickBlastStudentUI.Data;

namespace KickBlastStudentUI.Views;

public partial class SettingsView : UserControl
{
    private readonly Action<string> _status;

    public SettingsView(Action<string> status)
    {
        InitializeComponent();
        _status = status;
        LoadSettings();
    }

    private void LoadSettings()
    {
        var pricing = Db.GetPricing();
        BeginnerTextBox.Text = pricing.BeginnerFee.ToString();
        IntermediateTextBox.Text = pricing.IntermediateFee.ToString();
        EliteTextBox.Text = pricing.EliteFee.ToString();
        CompetitionTextBox.Text = pricing.CompetitionFee.ToString();
        CoachingTextBox.Text = pricing.CoachingRate.ToString();
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        if (!double.TryParse(BeginnerTextBox.Text, out var beginner) ||
            !double.TryParse(IntermediateTextBox.Text, out var intermediate) ||
            !double.TryParse(EliteTextBox.Text, out var elite) ||
            !double.TryParse(CompetitionTextBox.Text, out var competition) ||
            !double.TryParse(CoachingTextBox.Text, out var coaching))
        {
            _status("Invalid price value(s).");
            return;
        }

        Db.SavePricing(beginner, intermediate, elite, competition, coaching);
        _status("Settings saved.");
        MessageBox.Show("Pricing settings saved.");
    }
}
