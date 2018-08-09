using System.Collections.Generic;

namespace Ttl.CircuitBreakerManagementService.Impl
{
    class LogRequestFormatter
    {
        public static string FormatEssentials(CircuitBreakerState request)
        {
            string displayname;
            bool hasDisplayName= request.InformationalPropertyBag.TryGetValue("circuit.displayname", out displayname);  

            return string.Format("Circuit: {0} on {1}, pid: {2} status: {3}, msg: {4} from {5}.", request.CircuitBreakerId, request.MachineName, request.ProcessId, request.Status, 
                request.MessageSequenceNumber, (hasDisplayName) ? displayname : "no name.");
        }

        public static string FormatProperties(CircuitBreakerState request)
        {
            var pairs = new List<string>();
            foreach (var props in request.InformationalPropertyBag)
            {
                if (props.Key == "circuit.displayname") continue;
                if (props.Key == "circuit.calldurationlistinms") continue;
                if ((props.Key == "circuit.timestamps") && (props.Value.Length == 0)) continue; /// for diskspace reasons, we skip empty ones.

                pairs.Add(props.Key.Replace("circuit.","") + ":" + props.Value);
            }

            return string.Join(", ", pairs);
        }
    }
}
