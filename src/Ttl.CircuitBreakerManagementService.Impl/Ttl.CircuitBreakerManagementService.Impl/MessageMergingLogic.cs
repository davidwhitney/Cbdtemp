using Ttl.CircuitBreakerManagementService.Contract;

namespace Ttl.CircuitBreakerManagementService.Impl
{
    public class MessageMergingLogic
    {
        public CircuitBreakerState Merge(CircuitBreakerState updateMessage, CircuitBreakerState currentState)
        {
            //skip old messages, in case any have arrived (or get processed) out of order. 
            if (currentState.MessageSequenceNumber > updateMessage.MessageSequenceNumber) return currentState;

            if (currentState.MessageSequenceNumber == updateMessage.MessageSequenceNumber)
                return new CircuitBreakerState(currentState.CircuitBreakerId, currentState.MachineName, currentState.ProcessId)
                            {
                                MessageSequenceNumber = currentState.MessageSequenceNumber + 1000,
                                Status = currentState.Status,
                                LastReceivedStatus = updateMessage.Status,
                                InformationalPropertyBag = updateMessage.InformationalPropertyBag,
                                UpdateablePropertyBag = updateMessage.UpdateablePropertyBag
                            };

            var resultingState = new CircuitBreakerState(currentState.CircuitBreakerId, currentState.MachineName, currentState.ProcessId)
                {
                    MessageSequenceNumber = updateMessage.MessageSequenceNumber,
                    LastReceivedStatus = updateMessage.Status,
                    InformationalPropertyBag = updateMessage.InformationalPropertyBag,
                    UpdateablePropertyBag = updateMessage.UpdateablePropertyBag
                };


            switch (currentState.Status)
            {
                case CircuitBreakerStatus.ForcedClosed:
                case CircuitBreakerStatus.ForcedOpen:
                    resultingState.Status = currentState.Status;
                    break;
                case CircuitBreakerStatus.ForcedHalfOpen:
                    if ((updateMessage.Status == CircuitBreakerStatus.ForcedClosed) || (updateMessage.Status == CircuitBreakerStatus.ForcedOpen))
                        resultingState.Status = CircuitBreakerStatus.ForcedHalfOpen;
                    else
                        resultingState.Status = updateMessage.Status;

                    break;
                default:
                    resultingState.Status = updateMessage.Status;
                    break;
            }

            return resultingState;
        }
    }
}