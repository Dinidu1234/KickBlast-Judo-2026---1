using System.Windows.Controls;
using KickBlastStudentUI.Data;

namespace KickBlastStudentUI.Views;

public partial class DashboardView : UserControl
{
    private readonly Action<string> _status;

    public DashboardView(Action<string> status)
    {
        InitializeComponent();
        _status = status;
        LoadData();
    }

    private void LoadData()
    {
        AthleteCountText.Text = Db.GetAthleteCount().ToString();
        CalculationCountText.Text = Db.GetCalculationCount().ToString();
        UpdatedText.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        _status("Dashboard loaded.");
    }
}
