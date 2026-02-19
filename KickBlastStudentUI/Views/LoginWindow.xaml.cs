using System.Windows;
using KickBlastStudentUI.Data;

namespace KickBlastStudentUI.Views;

public partial class LoginWindow : Window
{
    public LoginWindow()
    {
        InitializeComponent();
        UsernameTextBox.Text = "rashiii";
    }

    private void Login_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var ok = Db.ValidateLogin(UsernameTextBox.Text.Trim(), PasswordBox.Password);
            if (ok)
            {
                var main = new MainWindow();
                main.Show();
                Close();
            }
            else
            {
                ErrorText.Visibility = Visibility.Visible;
            }
        }
        catch
        {
            ErrorText.Text = "Login failed. Please try again.";
            ErrorText.Visibility = Visibility.Visible;
        }
    }
}
