using System;
using System.Threading;
using Meadow;
using Meadow.Devices;
using Meadow.Hardware;
using Meadow.Peripherals.Sensors.Temperature;
using Meadow.Foundation.Sensors.Temperature;
using Meadow.Peripherals.Sensors.Atmospheric;

namespace Therm
{
    public class ThermApp : App<F7Micro, ThermApp>
    {
        AnalogTemperature _tempSensor;
        ClimateController _climateController;
        HvacController _hvacController;
        UXController _uxController;

        public ThermApp()
        {
            // setup our global hardware
            this.ConfigurePeripherals();

            // init our controllers
            this.InitializeControllers();

            // wire things up
            this.WireUpEventing();

            // get things spun up
            this.Start();
        }

        protected void ConfigurePeripherals()
        {
            _tempSensor = new AnalogTemperature(
                IOMap.AnalogTempSensor.Item1, IOMap.AnalogTempSensor.Item2,
                AnalogTemperature.KnownSensorType.TMP35);

            // subscribe to 1/4º C changes in temp
            this._tempSensor.Subscribe(new FilterableObserver<AtmosphericConditionChangeResult, AtmosphericConditions>(
                h => {
                    // probably update screen or something
                    Console.WriteLine($"Current Temp: {h.New.Temperature}ºC");
                    //_uxController.UpdateUX(h.New);
                },
                e => { return (Math.Abs(e.Delta.Temperature) > 0.25f); }));
        }

        protected void InitializeControllers()
        {
            // create our hvac controller. note that we create the outputs
            // that it manages here, because only it should be accessing them
            // directly.
            _hvacController = new HvacController(
                Device.CreateDigitalOutputPort(IOMap.Heater.Item2),
                Device.CreateDigitalOutputPort(IOMap.AirCon.Item2),
                Device.CreateDigitalOutputPort(IOMap.Fan.Item2)
                );

            _climateController = new ClimateController(
                this._hvacController,
                this._tempSensor
                );

            _uxController = new UXController();
        }

        protected void WireUpEventing()
        {
            // when there's UX input to change the climate, send it on to the
            // climate controller
            _uxController.ClimateModelChanged += (object sender, ClimateModelResult e) => {
                _climateController.SetDesiredClimate(e.Model);
            };

        }

        protected void Start()
        {
            _tempSensor.StartUpdating();
        }
    }
}
