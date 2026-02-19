namespace KickBlastStudentUI.Models;

public class Athlete
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Plan { get; set; } = "Beginner";
    public double CurrentWeight { get; set; }
    public double CategoryWeight { get; set; }
}
