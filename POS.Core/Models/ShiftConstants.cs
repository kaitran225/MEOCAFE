namespace POS.Core.Models;

public static class ShiftConstants
{
    public static TimeSpan Shift1In { get; } = TimeSpan.Parse("07:00:00");
    public static TimeSpan Shift2In { get; } = TimeSpan.Parse("12:00:00");
    public static TimeSpan Shift3In { get; } = TimeSpan.Parse("18:00:00");
    public static TimeSpan Shift4In { get; } = TimeSpan.Parse("22:00:00");
    public static TimeSpan Shift1Out { get; } = TimeSpan.Parse("12:00:00");
    public static TimeSpan Shift2Out { get; } = TimeSpan.Parse("18:00:00");
    public static TimeSpan Shift3Out { get; } = TimeSpan.Parse("22:00:00");
    public static TimeSpan Shift4Out { get; } = TimeSpan.Parse("07:00:00");

    public static string GetCheckinStatus(TimeSpan checkin, TimeSpan checkout)
    {
        var margin = TimeSpan.FromMinutes(30);
        if ((checkin >= Shift1In - margin && checkin <= Shift1In && checkout >= Shift1Out) ||
            (checkin >= Shift2In - margin && checkin <= Shift2In && checkout >= Shift2Out) ||
            (checkin >= Shift3In - margin && checkin <= Shift3In && checkout >= Shift3Out) ||
            (checkin >= Shift4In - margin && checkin <= Shift4In && checkout >= Shift4Out))
            return "ON_TIME";
        return "LATE";
    }
}
