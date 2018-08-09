using System;
using System.Timers;

namespace Ttl.CircuitBreakerManagementService.Impl
{
    public class TimedCleanupCircuitBreakerSet : ICollectGarbageBreakers
    {
        private readonly int _millisecondsUntilBreakerDeclaredQuiet;
        private readonly ICircuitBreakers _circuitBreakerSet;
        private Timer _quietBreakerCleanupTimer;

        public TimedCleanupCircuitBreakerSet(int quietBreakerCleanupPollIntervalInMs, int millisecondsUntilBreakerDeclaredQuiet, ICircuitBreakers circuitBreakerSet)
        {
            _millisecondsUntilBreakerDeclaredQuiet = millisecondsUntilBreakerDeclaredQuiet;
            _circuitBreakerSet = circuitBreakerSet;
            InitializeCleanupTimer(quietBreakerCleanupPollIntervalInMs);
        }

        private void InitializeCleanupTimer(int quietBreakerCleanupPollIntervalInMs)
        {
            _quietBreakerCleanupTimer = new Timer(quietBreakerCleanupPollIntervalInMs);
            _quietBreakerCleanupTimer.Elapsed += MarkBreakersForCleanup;
            _quietBreakerCleanupTimer.Start();
        }

        private void MarkBreakersForCleanup(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            DateTime expiryDateTime = SystemTime.Now().Subtract(new TimeSpan(0, 0, 0, 0, _millisecondsUntilBreakerDeclaredQuiet));
            _circuitBreakerSet.RemoveExpiredCircuitBreakers(expiryDateTime);
        }

        public void Dispose()
        {
            _quietBreakerCleanupTimer.Stop();
            _quietBreakerCleanupTimer.Elapsed -= MarkBreakersForCleanup;
            _quietBreakerCleanupTimer.Dispose();
        }
    }
}