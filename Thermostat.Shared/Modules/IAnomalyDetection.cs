﻿using TypeEdge.Attributes;
using TypeEdge.Modules.Endpoints;
using TypeEdge.Modules.Messages;
using ThermostatApplication.Messages;

namespace ThermostatApplication.Modules
{
    [TypeModule]
    public interface IAnomalyDetection 
    {
        Output<Anomaly> Anomaly { get; set; }
    }
}