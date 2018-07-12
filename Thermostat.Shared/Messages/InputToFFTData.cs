using TypeEdge.Modules.Messages;

namespace ThermostatApplication.Messages
{
    public class InputToFFTData : EdgeMessage
    {
        public double[] Values { get; set; }
        //Sampling rate in Hz
        public double SamplingRate { get; set; } 
        public string CorrelationID { get; set; }
    }
}