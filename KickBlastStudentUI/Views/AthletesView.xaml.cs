using System.Windows;
using System.Windows.Controls;
using KickBlastStudentUI.Data;
using KickBlastStudentUI.Models;

namespace KickBlastStudentUI.Views;

public partial class AthletesView : UserControl
{
    private readonly Action<string> _status;
    private int _selectedId;

    public AthletesView(Action<string> status)
    {
        InitializeComponent();
        _status = status;
        PlanComboBox.SelectedIndex = 0;
        SearchPlanComboBox.SelectedIndex = 0;
        LoadGrid();
    }

    private void LoadGrid()
    {
        var plan = ((ComboBoxItem)SearchPlanComboBox.SelectedItem).Content.ToString() ?? "All";
        AthletesGrid.ItemsSource = Db.GetAthletes(SearchTextBox.Text.Trim(), plan);
        _status("Athletes loaded.");
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (!double.TryParse(CurrentWeightTextBox.Text, out var current) || !double.TryParse(CategoryWeightTextBox.Text, out var category))
            {
                _status("Invalid weight values.");
                return;
            }

            var athlete = new Athlete
            {
                Id = _selectedId,
                Name = NameTextBox.Text.Trim(),
                Plan = ((ComboBoxItem)PlanComboBox.SelectedItem).Content.ToString() ?? "Beginner",
                CurrentWeight = current,
                CategoryWeight = category
            };

            Db.SaveAthlete(athlete);
            LoadGrid();
            ClearForm();
            _status("Athlete saved.");
        }
        catch
        {
            _status("Failed to save athlete.");
        }
    }

    private void Delete_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedId == 0)
        {
            _status("Please select an athlete.");
            return;
        }

        if (MessageBox.Show("Delete selected athlete?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        {
            Db.DeleteAthlete(_selectedId);
            LoadGrid();
            ClearForm();
            _status("Athlete deleted.");
        }
    }

    private void Clear_Click(object sender, RoutedEventArgs e)
    {
        ClearForm();
        _status("Form cleared.");
    }

    private void Search_Click(object sender, RoutedEventArgs e)
    {
        LoadGrid();
    }

    private void AthletesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (AthletesGrid.SelectedItem is Athlete athlete)
        {
            _selectedId = athlete.Id;
            NameTextBox.Text = athlete.Name;
            SelectCombo(PlanComboBox, athlete.Plan);
            CurrentWeightTextBox.Text = athlete.CurrentWeight.ToString();
            CategoryWeightTextBox.Text = athlete.CategoryWeight.ToString();
        }
    }

    private void ClearForm()
    {
        _selectedId = 0;
        NameTextBox.Text = "";
        PlanComboBox.SelectedIndex = 0;
        CurrentWeightTextBox.Text = "";
        CategoryWeightTextBox.Text = "";
        AthletesGrid.SelectedItem = null;
    }

    private void SelectCombo(ComboBox comboBox, string value)
    {
        foreach (ComboBoxItem item in comboBox.Items)
        {
            if (item.Content?.ToString() == value)
            {
                comboBox.SelectedItem = item;
                return;
            }
        }
    }
}
