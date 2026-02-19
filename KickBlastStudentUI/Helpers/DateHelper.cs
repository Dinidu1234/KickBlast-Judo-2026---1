namespace KickBlastStudentUI.Helpers;

public static class DateHelper
{
    public static DateTime GetSecondSaturday(DateTime date)
    {
        var firstDay = new DateTime(date.Year, date.Month, 1);
        var firstSaturdayOffset = ((int)DayOfWeek.Saturday - (int)firstDay.DayOfWeek + 7) % 7;
        var firstSaturday = firstDay.AddDays(firstSaturdayOffset);
        return firstSaturday.AddDays(7);
    }
}
