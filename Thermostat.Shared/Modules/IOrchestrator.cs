﻿using TypeEdge.Attributes;
using TypeEdge.Modules.Endpoints;
using TypeEdge.Twins;
using ThermostatApplication.Messages;
using ThermostatApplication.Twins;

namespace ThermostatApplication.Modules
{
    [TypeModule]
    public interface IOrchestrator
    {
        Output<Temperature> Sampling { get; set; }
        Output<Temperature> Detection { get; set; }
        Output<GraphData> Visualization { get; set; }

        ModuleTwin<OrchestratorTwin> Twin { get; set; }
    }
}