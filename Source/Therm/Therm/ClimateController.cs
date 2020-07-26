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
        // internals
        protected HvacController _hvacController;
        //protected AnalogTemperature _tempSensor;
        protected int _standbyDuration = 4000; // 30 second on/off intervals (4 for testing)
        protected ClimateModel _desiredClimateOperation;

        // internal thread lock
        private object _climateMaintLock = new object();
        private CancellationTokenSource _climateMaintCancelToken;

        // state
        public bool IsRunning { get; protected set; } = false;
        public Mode CurrentMode { get; protected set; }


        public ClimateController(
            //HvacController hvacController,
            //AnalogTemperature temperatureSensor
            )
        {
            _hvacController = new HvacController(
                ThermApp.Device.CreateDigitalOutputPort(IOMap.HVac.HeaterPin),
                ThermApp.Device.CreateDigitalOutputPort(IOMap.HVac.FanPin),
                ThermApp.Device.CreateDigitalOutputPort(IOMap.HVac.AirConPin)
            );

            // when the climate model changes, make sure to update the HVAC
            // state
            ThermApp.ModelManager.Subscribe(new FilterableChangeObserver<ClimateModelChangeResult, ClimateModel>(
                h => {
                    Console.WriteLine("ClimateController: Climate model changed, updating hvac.");
                    _desiredClimateOperation = h.New;
                    UpdateClimateIntent();
                }
            ));
        }

        /// <summary>
        /// Stop maintaining climate. Will shut the HVAC system down.
        ///
        /// TODO: this probably isn't needed. 
        /// </summary>
        public void TurnOff()
        {
            lock (_climateMaintLock) {
                if (!IsRunning) return;

                if (_climateMaintCancelToken != null) {
                    _climateMaintCancelToken.Cancel();
                }

                // state muh-cheen
                IsRunning = false;
                CurrentMode = Mode.Off;
                _hvacController.TurnAllOff();
            }
        }


        /// <summary>
        /// Spins up a thread that checks the temp and 
        /// </summary>
        protected void UpdateClimateIntent()
        {
            // thread safety
            lock (_climateMaintLock) {
                bool skipCancel = false;

                // if we're not changing the mode, and we're
                // already running, it'll respond to the change in desired temp
                // and ambient temp in the PID loopp.
                if (_desiredClimateOperation.HvacOperatingMode == this.CurrentMode) {
                    if (IsRunning) { return; }
                } // otherwise, if we've got an idle loop, cancel it. 
                else {
                    if (IsRunning) {
                        _climateMaintCancelToken.Cancel();
                        skipCancel = true; // we'll use this later.
                    }
                }

                // state muh-cheen
                IsRunning = true;

                _climateMaintCancelToken = new CancellationTokenSource();
                CancellationToken ct = _climateMaintCancelToken.Token;

                Task.Factory.StartNew(async (wasRunning) => {
                    
                    while (true) {
                        // if it was runing, and we got here, we don't actually
                        // want to honor the cancelation, because it was meant
                        // for the last loop.
                        if (!skipCancel && ct.IsCancellationRequested) {
                                break;
                        }
                        // reset so we check for cancel next time through the loop
                        if (skipCancel) { skipCancel = false; }


                        // TODO: PID and HVAC logic go here. make sure
                        // to check against the current climate model


                        switch (CurrentMode) {
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
                }, _climateMaintCancelToken.Token);
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