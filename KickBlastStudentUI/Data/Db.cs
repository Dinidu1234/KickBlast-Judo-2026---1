using System.IO;
using Microsoft.Data.Sqlite;
using KickBlastStudentUI.Models;

namespace KickBlastStudentUI.Data;

public static class Db
{
    private static readonly string ConnectionString;

    static Db()
    {
        var dataDir = Path.Combine(AppContext.BaseDirectory, "Data");
        Directory.CreateDirectory(dataDir);
        var dbPath = Path.Combine(dataDir, "kickblast_student.db");
        ConnectionString = $"Data Source={dbPath}";
    }

    public static void Initialize()
    {
        CreateTables();
        SeedData();
    }

    private static void CreateTables()
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = @"
CREATE TABLE IF NOT EXISTS Users (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Username TEXT UNIQUE,
    Password TEXT
);
CREATE TABLE IF NOT EXISTS Athletes (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT,
    Plan TEXT,
    CurrentWeight REAL,
    CategoryWeight REAL
);
CREATE TABLE IF NOT EXISTS MonthlyCalculations (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Date TEXT,
    AthleteName TEXT,
    Plan TEXT,
    Competitions INTEGER,
    CoachingHours REAL,
    TrainingCost REAL,
    CoachingCost REAL,
    CompetitionCost REAL,
    TotalCost REAL,
    WeightMessage TEXT,
    SecondSaturday TEXT
);
CREATE TABLE IF NOT EXISTS Pricing (
    Id INTEGER PRIMARY KEY,
    BeginnerFee REAL,
    IntermediateFee REAL,
    EliteFee REAL,
    CompetitionFee REAL,
    CoachingRate REAL
);";
        command.ExecuteNonQuery();
    }

    private static void SeedData()
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        var userCount = Convert.ToInt32(new SqliteCommand("SELECT COUNT(*) FROM Users", connection).ExecuteScalar());
        if (userCount == 0)
        {
            var userCmd = connection.CreateCommand();
            userCmd.CommandText = "INSERT INTO Users (Username, Password) VALUES ('rashiii','123456');";
            userCmd.ExecuteNonQuery();
        }

        var athleteCount = Convert.ToInt32(new SqliteCommand("SELECT COUNT(*) FROM Athletes", connection).ExecuteScalar());
        if (athleteCount == 0)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
INSERT INTO Athletes (Name, Plan, CurrentWeight, CategoryWeight) VALUES
('Kamal Perera','Beginner',66.5,67),
('Nethmi Silva','Intermediate',58.2,57),
('Sahan Jayasekara','Elite',74.0,73),
('Ayesha Fernando','Beginner',51.0,52),
('Ravindu Gunathilaka','Intermediate',80.0,81),
('Dilini Ranasinghe','Elite',62.3,63);
";
            cmd.ExecuteNonQuery();
        }

        var pricingCount = Convert.ToInt32(new SqliteCommand("SELECT COUNT(*) FROM Pricing", connection).ExecuteScalar());
        if (pricingCount == 0)
        {
            var priceCmd = connection.CreateCommand();
            priceCmd.CommandText = @"INSERT INTO Pricing (Id, BeginnerFee, IntermediateFee, EliteFee, CompetitionFee, CoachingRate)
VALUES (1, 2000, 3000, 4500, 1500, 1200);";
            priceCmd.ExecuteNonQuery();
        }
    }

    public static bool ValidateLogin(string username, string password)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM Users WHERE Username = @u AND Password = @p";
        command.Parameters.AddWithValue("@u", username);
        command.Parameters.AddWithValue("@p", password);
        return Convert.ToInt32(command.ExecuteScalar()) > 0;
    }

    public static List<Athlete> GetAthletes(string search = "", string plan = "All")
    {
        var list = new List<Athlete>();
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = @"SELECT Id, Name, Plan, CurrentWeight, CategoryWeight
FROM Athletes
WHERE Name LIKE @search AND (@plan = 'All' OR Plan = @plan)
ORDER BY Name";
        command.Parameters.AddWithValue("@search", $"%{search}%");
        command.Parameters.AddWithValue("@plan", plan);
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new Athlete
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Plan = reader.GetString(2),
                CurrentWeight = reader.GetDouble(3),
                CategoryWeight = reader.GetDouble(4)
            });
        }
        return list;
    }

    public static void SaveAthlete(Athlete athlete)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        var command = connection.CreateCommand();
        if (athlete.Id == 0)
        {
            command.CommandText = "INSERT INTO Athletes (Name, Plan, CurrentWeight, CategoryWeight) VALUES (@n, @p, @cw, @cat)";
        }
        else
        {
            command.CommandText = "UPDATE Athletes SET Name=@n, Plan=@p, CurrentWeight=@cw, CategoryWeight=@cat WHERE Id=@id";
            command.Parameters.AddWithValue("@id", athlete.Id);
        }

        command.Parameters.AddWithValue("@n", athlete.Name);
        command.Parameters.AddWithValue("@p", athlete.Plan);
        command.Parameters.AddWithValue("@cw", athlete.CurrentWeight);
        command.Parameters.AddWithValue("@cat", athlete.CategoryWeight);
        command.ExecuteNonQuery();
    }

    public static void DeleteAthlete(int id)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        var command = new SqliteCommand("DELETE FROM Athletes WHERE Id=@id", connection);
        command.Parameters.AddWithValue("@id", id);
        command.ExecuteNonQuery();
    }

    public static (double BeginnerFee, double IntermediateFee, double EliteFee, double CompetitionFee, double CoachingRate) GetPricing()
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        var cmd = new SqliteCommand("SELECT BeginnerFee, IntermediateFee, EliteFee, CompetitionFee, CoachingRate FROM Pricing WHERE Id = 1", connection);
        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return (reader.GetDouble(0), reader.GetDouble(1), reader.GetDouble(2), reader.GetDouble(3), reader.GetDouble(4));
        }

        return (2000, 3000, 4500, 1500, 1200);
    }

    public static void SavePricing(double beginner, double intermediate, double elite, double competition, double coaching)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        var cmd = connection.CreateCommand();
        cmd.CommandText = @"UPDATE Pricing
SET BeginnerFee=@b, IntermediateFee=@i, EliteFee=@e, CompetitionFee=@c, CoachingRate=@r
WHERE Id=1";
        cmd.Parameters.AddWithValue("@b", beginner);
        cmd.Parameters.AddWithValue("@i", intermediate);
        cmd.Parameters.AddWithValue("@e", elite);
        cmd.Parameters.AddWithValue("@c", competition);
        cmd.Parameters.AddWithValue("@r", coaching);
        cmd.ExecuteNonQuery();
    }

    public static int GetAthleteCount() => GetCount("Athletes");
    public static int GetCalculationCount() => GetCount("MonthlyCalculations");

    private static int GetCount(string table)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        return Convert.ToInt32(new SqliteCommand($"SELECT COUNT(*) FROM {table}", connection).ExecuteScalar());
    }

    public static void SaveCalculation(MonthlyCalculation calculation)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        var cmd = connection.CreateCommand();
        cmd.CommandText = @"INSERT INTO MonthlyCalculations
(Date, AthleteName, Plan, Competitions, CoachingHours, TrainingCost, CoachingCost, CompetitionCost, TotalCost, WeightMessage, SecondSaturday)
VALUES (@d, @a, @p, @comp, @hours, @train, @coach, @cc, @total, @wm, @ss)";
        cmd.Parameters.AddWithValue("@d", calculation.Date);
        cmd.Parameters.AddWithValue("@a", calculation.AthleteName);
        cmd.Parameters.AddWithValue("@p", calculation.Plan);
        cmd.Parameters.AddWithValue("@comp", calculation.Competitions);
        cmd.Parameters.AddWithValue("@hours", calculation.CoachingHours);
        cmd.Parameters.AddWithValue("@train", calculation.TrainingCost);
        cmd.Parameters.AddWithValue("@coach", calculation.CoachingCost);
        cmd.Parameters.AddWithValue("@cc", calculation.CompetitionCost);
        cmd.Parameters.AddWithValue("@total", calculation.TotalCost);
        cmd.Parameters.AddWithValue("@wm", calculation.WeightMessage);
        cmd.Parameters.AddWithValue("@ss", calculation.SecondSaturday);
        cmd.ExecuteNonQuery();
    }

    public static List<MonthlyCalculation> GetHistory(string athlete = "All", int month = 0, int year = 0)
    {
        var list = new List<MonthlyCalculation>();
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = @"SELECT Id, Date, AthleteName, Plan, Competitions, CoachingHours, TrainingCost, CoachingCost, CompetitionCost, TotalCost, WeightMessage, SecondSaturday
FROM MonthlyCalculations
WHERE (@athlete = 'All' OR AthleteName = @athlete)
AND (@month = 0 OR CAST(strftime('%m', Date) AS INTEGER) = @month)
AND (@year = 0 OR CAST(strftime('%Y', Date) AS INTEGER) = @year)
ORDER BY Date DESC";
        cmd.Parameters.AddWithValue("@athlete", athlete);
        cmd.Parameters.AddWithValue("@month", month);
        cmd.Parameters.AddWithValue("@year", year);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new MonthlyCalculation
            {
                Id = reader.GetInt32(0),
                Date = reader.GetString(1),
                AthleteName = reader.GetString(2),
                Plan = reader.GetString(3),
                Competitions = reader.GetInt32(4),
                CoachingHours = reader.GetDouble(5),
                TrainingCost = reader.GetDouble(6),
                CoachingCost = reader.GetDouble(7),
                CompetitionCost = reader.GetDouble(8),
                TotalCost = reader.GetDouble(9),
                WeightMessage = reader.GetString(10),
                SecondSaturday = reader.GetString(11)
            });
        }

        return list;
    }
}
