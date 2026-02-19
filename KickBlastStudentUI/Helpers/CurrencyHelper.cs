using System.Globalization;

namespace KickBlastStudentUI.Helpers;

public static class CurrencyHelper
{
    public static string ToLkr(double amount)
    {
        return "LKR " + amount.ToString("N2", CultureInfo.InvariantCulture);
    }
}
