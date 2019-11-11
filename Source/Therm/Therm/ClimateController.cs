using System;
using System.Threading;
using System.Threading.Tasks;
using Meadow;
using Meadow.Foundation.Sensors.Temperature;

namespace Therm
{
    /// <summary>
    /// Controller responsible for checking the ambient temp and doing the
    /// right thing with the HVAC system, depending on what the user wants.
    ///
    /// Call SetDesiredTemp() to put it to work.
    /// </summary>
    public class ClimateController
    {
        protected HvacController _hvacController;
        protected AnalogTemperature _tempSensor;
        protected int _standbyDuration = 30000; // 30 second on/off intervals
        protected ClimateModel _desiredClimate;

        public bool IsRunning { get; protected set; } = false;
        public Mode CurrentMode { get; protected set; }

        public ClimateController(
            HvacController hvacController,
            AnalogTemperature temperatureSensor
            )
        {
            this._hvacController = hvacController;
            this._tempSensor = temperatureSensor;
        }

        public void SetDesiredClimate(ClimateModel model)
        {
            this._desiredClimate = model;
            this.CurrentMode = model.HvacOperatingMode;
            // if we're not already running, call running.
            // if we are running, it'll pick up the new changes during the cycle
            StartMaintainingClimate();
        }

        public void TurnOff()
        {
            lock (_lock) {
                if (!IsRunning) return;

                if (SamplingTokenSource != null) {
                    SamplingTokenSource.Cancel();
                }

                // state muh-cheen
                IsRunning = false;
                this.CurrentMode = Mode.Off;
                this._hvacController.TurnAllOff();
            }
        }

        // internal thread lock
        private object _lock = new object();
        private CancellationTokenSource SamplingTokenSource;

        /// <summary>
        /// Spins up a thread that checks the temp and 
        /// </summary>
        protected void StartMaintainingClimate()
        {
            // thread safety
            lock (_lock) {
                if (IsRunning) return;

                // state muh-cheen
                IsRunning = true;

                SamplingTokenSource = new CancellationTokenSource();
                CancellationToken ct = SamplingTokenSource.Token;

                Task.Factory.StartNew(async () => {
                    while (true) {
                        // TODO: someone please review; is this the correct
                        // place to do this?
                        // check for cancel (doing this here instead of 
                        // while(!ct.IsCancellationRequested), so we can perform 
                        // cleanup
                        if (ct.IsCancellationRequested) {
                            break;
                        }

                        // TODO: use this in PID. temp sensor is already spinning
                        //float temp = _tempSensor.Temperature;

                        // TODO: PID and HVAC logic go here.


                        switch (this.CurrentMode) {
                            case Mode.Auto:
                                break;
                            case Mode.Cool:
                                break;
                            case Mode.FanOnly:
                                _hvacController.TurnAirConOff();
                                _hvacController.TurnHeatOff();
                                _hvacController.TurnFanOn();
                                break;
                            case Mode.Heat:
                                break;
                            case Mode.Off:
                                _hvacController.TurnAllOff();
                                break;
                        }

                        // sleep for the appropriate interval
                        await Task.Delay(_standbyDuration);
                    }
                }, SamplingTokenSource.Token);
            }
        }

        public enum Mode
        {
            /// <summary>
            /// No heating, cooling, or fan. 
            /// </summary>
            Off,
            /// <summary>
            /// Automatically heat or cool, to get to the desired temperature.
            /// </summary>
            Auto,
            /// <summary>
            /// Heat to desired temperature.
            /// </summary>
            Heat,
            /// <summary>
            /// Cool to desired temperature.
            /// </summary>
            Cool,
            /// <summary>
            /// Only run the fan.
            /// </summary>
            FanOnly
        }
    }
}
