namespace Utils.Time
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime Now() => DateTime.Now;

        public DateTime NowUtc() => DateTime.UtcNow;

        public DateTimeOffset NowOffset() => DateTimeOffset.Now;

        public DateTimeOffset NowUtcOffset() => DateTimeOffset.UtcNow;
    }
}
