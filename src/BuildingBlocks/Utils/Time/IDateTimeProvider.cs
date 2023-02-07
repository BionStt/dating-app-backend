namespace Utils.Time
{
    //The purpose of this interface is to make unit testing easier in dates.
    public interface IDateTimeProvider
    {
        DateTime Now();
        DateTime NowUtc();
        DateTimeOffset NowOffset();
        DateTimeOffset NowUtcOffset();
    }
}
