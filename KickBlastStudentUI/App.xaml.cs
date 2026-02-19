using System.Windows;
using KickBlastStudentUI.Data;
using KickBlastStudentUI.Views;

namespace KickBlastStudentUI;

public partial class App : Application
{
    private void Application_Startup(object sender, StartupEventArgs e)
    {
        Db.Initialize();
        var login = new LoginWindow();
        login.Show();
    }
}
