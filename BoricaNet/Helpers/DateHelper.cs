namespace BoricaNet.Helpers;

internal static class DateHelper
{
    public static string GetTimezoneOffset()
    {
        var offset = TimeZoneInfo.Local.GetUtcOffset(DateTime.Now);
        var offsetString = offset.ToString("hh");
        return (offset < TimeSpan.Zero ? "" : "+") + offsetString.PadLeft(2, '0');
    }
    
    public static string FormatDateForBorica(DateTime date)
    {
        const string pattern = "yyyyMMddHHmmss";
        var utcDate = date.ToUniversalTime();
        return utcDate.ToString(pattern);
    }
}