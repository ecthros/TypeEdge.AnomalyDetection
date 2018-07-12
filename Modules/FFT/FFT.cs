using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using TypeEdge.Modules;
using TypeEdge.Modules.Endpoints;
using TypeEdge.Modules.Enums;
using TypeEdge.Modules.Messages;
using TypeEdge.Twins;
using Microsoft.Extensions.Configuration;
using Python.Runtime;
using FFT;
using ThermostatApplication.Messages;
using ThermostatApplication.Twins;
using ThermostatApplication.Modules;



namespace Modules
{
    public class FFT : EdgeModule, IDisposable
    {
        private Py.GILState _state;
        private dynamic _sys;
        private dynamic _np;

        private string _code;
        private SingleThreadTaskScheduler _pythonTaskScheduler;
        public override CreationResult Configure(IConfigurationRoot configuration)
        {
            var cts = new CancellationTokenSource();
            _pythonTaskScheduler = new SingleThreadTaskScheduler(cts.Token);


            _pythonTaskScheduler.Schedule(() =>
            {
                try
                {
                    _state = Py.GIL();
                    _sys = Py.Import("sys");
                    _np = Py.Import("numpy");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            });
            _pythonTaskScheduler.Start();

            _code = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "FFTCalculator.py"));

            return string.IsNullOrEmpty(_code) ? CreationResult.Error : CreationResult.Ok;
        }

        public FFT(IOrchestrator proxy)
        {
            proxy.SignalData.Subscribe(this, async msg =>
            {
                Console.WriteLine("Processing new message in FFT module");
                double[][] fftresult = {};


                await _pythonTaskScheduler.Schedule(() =>
                {
                    try
                    {
                        //TODO: run your python code here.

                        PythonEngine.RunSimpleString(_code);

                        dynamic t = _sys.FFTCalculator;
                        fftresult = (double[][]) t.fft_calculate(msg.Values, msg.SamplingRate);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                });


                await Output.PublishAsync(new OutputFromFFTData()
                {
                    CorrelationID = msg.CorrelationID,
                    Values = fftresult
                });
                Console.WriteLine("TypeEdgeModule3: Generated Message");

                return MessageResult.Ok;
            });
        }

        public Output<OutputFromFFTData> Output { get; set; }
        public ModuleTwin<FFTModuleTwin> Twin { get; set; }

        public new void Dispose()
        {
            _pythonTaskScheduler?.Schedule(() => { _state?.Dispose(); });
            base.Dispose();
        }
    }
}