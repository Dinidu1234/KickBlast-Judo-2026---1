using System.Windows;
using System.Windows.Controls;
using KickBlastStudentUI.Views;

namespace KickBlastStudentUI;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        LoadView("Dashboard");
    }

    private void LoadView(string view)
    {
        UserControl control;
        switch (view)
        {
            case "Athletes":
                PageTitleText.Text = "Athletes";
                control = new AthletesView(SetStatusMessage);
                break;
            case "Calculator":
                PageTitleText.Text = "Monthly Fee Calculator";
                control = new CalculatorView(SetStatusMessage);
                break;
            case "History":
                PageTitleText.Text = "History";
                control = new HistoryView(SetStatusMessage);
                break;
            case "Settings":
                PageTitleText.Text = "Settings";
                control = new SettingsView(SetStatusMessage);
                break;
            default:
                PageTitleText.Text = "Dashboard";
                control = new DashboardView(SetStatusMessage);
                break;
        }

        MainContent.Content = control;
    }

    private void SetStatusMessage(string message)
    {
        StatusText.Text = message;
    }

    private void DashboardButton_Click(object sender, RoutedEventArgs e) => LoadView("Dashboard");
    private void AthletesButton_Click(object sender, RoutedEventArgs e) => LoadView("Athletes");
    private void CalculatorButton_Click(object sender, RoutedEventArgs e) => LoadView("Calculator");
    private void HistoryButton_Click(object sender, RoutedEventArgs e) => LoadView("History");
    private void SettingsButton_Click(object sender, RoutedEventArgs e) => LoadView("Settings");
}
