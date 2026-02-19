namespace KickBlastStudentUI.Helpers;

public static class ValidationHelper
{
    public static bool TryParsePositiveInt(string value, out int result)
    {
        return int.TryParse(value, out result) && result >= 0;
    }

    public static bool TryParseDoubleInRange(string value, double min, double max, out double result)
    {
        return double.TryParse(value, out result) && result >= min && result <= max;
    }
}
