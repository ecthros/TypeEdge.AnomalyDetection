using TypeEdge.Modules.Messages;

namespace ThermostatApplication.Messages
{
    public class OutputFromFFTData : EdgeMessage
    {
        public double[][] Values { get; set; }
        public string CorrelationID { get; set; }
    }
}