using System;
using System.Threading;
using Meadow;
using Meadow.Devices;
using Meadow.Hardware;
using Meadow.Peripherals.Sensors.Temperature;
using Meadow.Foundation.Sensors.Temperature;
using Meadow.Peripherals.Sensors.Atmospheric;
using System.Threading.Tasks;

namespace Therm
{
    public class ThermApp : App<F7Micro, ThermApp>
    {
        AnalogTemperature _tempSensor;
        ClimateController _climateController;
        HvacController _hvacController;
        UXController _uxController;

        public static ClimateModelManager ModelManager { get => ClimateModelManager.Instance; }

        public ThermApp()
        {
            // setup our global hardware
            this.ConfigurePeripherals();

            // init our controllers
            this.InitializeControllers();

            // wire things up
            this.WireUpEventing();

            // get things spun up
            this.Start().Wait();
        }

        protected void ConfigurePeripherals()
        {
            _tempSensor = new AnalogTemperature(
                IOMap.AnalogTempSensor.Item1, IOMap.AnalogTempSensor.Item2,
                AnalogTemperature.KnownSensorType.TMP35);
        }

        protected void InitializeControllers()
        {
            _climateController = new ClimateController();
            _uxController = new UXController();
        }


        /// <summary>
        /// Glues things together with the subscribers
        /// </summary>
        protected void WireUpEventing()
        {
            // subscribe to 1/4º C changes in temp
            this._tempSensor.Subscribe(new FilterableObserver<AtmosphericConditionChangeResult, AtmosphericConditions>(
                h => {
                    // probably update screen or something
                    Console.WriteLine($"Current Temperature: {h.New.Temperature}ºC");
                    ModelManager.UpdateAmbientTemp(h.New.Temperature);
                },
                e => { return (Math.Abs(e.Delta.Temperature) > 0.25f); }));
        }

        /// <summary>
        /// Kicks off the app. Starts by doing a temp read and then spins
        /// up the sensor updating and such.
        /// </summary>
        /// <returns></returns>
        protected async Task Start()
        {
            // take an initial reading of the temp
            Console.WriteLine("Start");
            var conditions = await _tempSensor.Read();
            Console.WriteLine($"Initial temp: {conditions.Temperature}");
            ModelManager.UpdateAmbientTemp(conditions.Temperature);
            //ModelManager.UpdateAmbientTemp(20f);

            //
            Console.WriteLine("Starting up the temp sensor.");
            _tempSensor.StartUpdating(standbyDuration: 1000);

            Console.WriteLine("Temp sensor spinning");
        }
    }
}
