namespace Backend.Utils
{
    public class TimeUtils
    {
        public static DateTime FromUnixTime(long unixTimeSeconds)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeSeconds).ToLocalTime();
            return dateTime;
        }
    }
}
