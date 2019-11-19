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

            // subscribe to 1/4º C changes in temp
            this._tempSensor.Subscribe(new FilterableObserver<AtmosphericConditionChangeResult, AtmosphericConditions>(
                h => {
                    // probably update screen or something
                    Console.WriteLine($"Current Temp: {h.New.Temperature}ºC");
                    ModelManager.UpdateAmbientTemp(h.New.Temperature);
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


        /// <summary>
        /// Glues things together with the subscribers
        /// </summary>
        protected void WireUpEventing()
        {
            // when there's UX input to change the climate, send it on to the
            // climate controller
            ModelManager.Subscribe(new FilterableObserver<ClimateModelChangeResult, ClimateModel> (
                h => {
                    _climateController.SetDesiredClimate(h.New);
                }
            ));

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
            //var conditions = await _tempSensor.Read();
            //Console.WriteLine($"Initial temp: {conditions.Temperature}");
            //ModelManager.UpdateAmbientTemp(conditions.Temperature);
            ModelManager.UpdateAmbientTemp(20f);

            //
            Console.WriteLine("Starting up the temp sensor.");
            _tempSensor.StartUpdating();
        }
    }
}
