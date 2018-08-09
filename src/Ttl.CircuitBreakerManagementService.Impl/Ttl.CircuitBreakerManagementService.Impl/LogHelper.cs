using Ttl.CircuitBreakerManagementService.Contract;
using log4net;

namespace Ttl.CircuitBreakerManagementService.Impl
{
    static class LogHelper
    {
        public static void Log(ILog logger, CircuitBreakerState request, bool isNew)
        {
            string logEntry = LogRequestFormatter.FormatEssentials(request);
            string props = LogRequestFormatter.FormatProperties(request);
            string message = logEntry + " " + props;
            message = isNew ? "(NEW) " + message : message;

            if ((request.Status == CircuitBreakerStatus.Open) || (request.Status == CircuitBreakerStatus.ForcedOpen) || (request.Status == CircuitBreakerStatus.ForcedClosed) || (request.Status == CircuitBreakerStatus.ForcedHalfOpen))
            {
                logger.Warn(message);
            }
            else
                logger.Info(message);
        }
    }
}
