using System;

namespace Ttl.CircuitBreakerManagementService.Impl
{
    public static class SystemTime // Thanks Ayende, http://ayende.com/blog/3408/dealing-with-time-in-tests
    {
        public static Func<DateTime> Now = () => DateTime.Now;

        public static DateTime Is(int millisecond)
        {
            var now = Now();
            var dateTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, millisecond);
            Now = () => dateTime;
            return dateTime;
        }

        public static DateTime Is(int second, int millisecond)
        {
            var now = Now();
            var dateTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, second, millisecond);
            Now = () => dateTime;
            return dateTime;
        }

        public static void IsNow()
        {
            Now = () => DateTime.Now;
        }
    }

    //public class TimeIs : IDisposable
    //{
    //    private readonly Func<DateTime> _currentFunc;


    //    public TimeIs(int hour, int minute, int second, int millisecond)
    //    {
    //        _currentFunc = SystemTime.Now;
    //        SystemTime.Now = () =>
    //            {
    //                var now = _currentFunc();
    //                return new DateTime(now.Year, now.Month, now.Day, hour, minute, second, millisecond);
    //            };
    //    }

    //    public TimeIs(int minute, int second, int millisecond)
    //    {
    //        _currentFunc = SystemTime.Now;
    //        SystemTime.Now = () =>
    //            {
    //                var now = _currentFunc();
    //                return new DateTime(now.Year, now.Month, now.Day, now.Hour, minute, second, millisecond);
    //            };
    //    }

    //    public TimeIs(int second, int millisecond)
    //    {
    //        _currentFunc = SystemTime.Now;
    //        SystemTime.Now = () =>
    //            {
    //                var now = _currentFunc();
    //                return new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, second, millisecond);
    //            };
    //    }

    //    public TimeIs(int millisecond)
    //    {
    //        _currentFunc = SystemTime.Now;
    //        SystemTime.Now = () =>
    //            {
    //                var now = _currentFunc();
    //                return new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, millisecond);
    //            };
    //    }


    //    public void Dispose()
    //    {
    //        SystemTime.Now = _currentFunc;
    //    }
    //}
}