namespace EDM.Core.Models;

public class DayInfo
{
    public DateTime Date { get; set; }

    /// <summary>
    /// Daily energy produced
    /// </summary>
    public double Energy { get; set; }

    /// <summary>
    /// Price for the day
    /// </summary>
    public double Price { get; set; }
}