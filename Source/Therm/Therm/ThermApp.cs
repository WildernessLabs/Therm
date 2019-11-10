using System;
using System.Threading;
using Meadow;
using Meadow.Devices;
using Meadow.Hardware;
using Meadow.Peripherals.Sensors.Temperature;
using Meadow.Foundation.Sensors.Temperature;

namespace Therm
{
    public class ThermApp : App<F7Micro, ThermApp>
    {
        AnalogTemperature _tempSensor;
        ClimateController _climateController;
        HvacController _hvacController;

        IPin _heaterPin = Device.Pins.D00;
        IPin _airConPin = Device.Pins.D01;
        IPin _fanPin    = Device.Pins.D02;

        public ThermApp()
        {
            // setup our global hardware
            this.ConfigurePeripherals();

            // init our controllers
            this.InitializeControllers();

            // get things spun up
            this.Start();
        }

        protected void ConfigurePeripherals()
        {
            _tempSensor = new AnalogTemperature(
                Device, Device.Pins.A00,
                AnalogTemperature.KnownSensorType.TMP35);

            // subscribe to 1/4º C changes in temp
            this._tempSensor.Subscribe(new FilterableObserver<FloatChangeResult, float>(
                h => {
                    // probably update screen or something
                },
                e => { return (Math.Abs(e.Delta) > 0.25f); }));
        }

        protected void InitializeControllers()
        {
            // create our hvac controller. note that we create the outputs
            // that it manages here, because only it should be accessing them
            // directly.
            _hvacController = new HvacController(
                Device.CreateDigitalOutputPort(_heaterPin),
                Device.CreateDigitalOutputPort(_airConPin),
                Device.CreateDigitalOutputPort(_fanPin)
                );

            _climateController = new ClimateController(
                this._hvacController,
                this._tempSensor
                );
        }

        protected void Start()
        {
            _tempSensor.StartUpdating();
        }
    }
}
