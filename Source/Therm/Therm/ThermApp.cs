using System;
using Meadow;
using Meadow.Devices;
using Meadow.Foundation.Sensors.Temperature;
using Meadow.Peripherals.Sensors.Atmospheric;
using System.Threading.Tasks;

namespace Therm
{
    public class ThermApp : App<F7Micro, ThermApp>
    {
        AnalogTemperature temperatureSensor;
        ClimateController climateController;
        HvacController hvacController;
        UXController uxController;

        public static ClimateModelManager ModelManager { get => ClimateModelManager.Instance; }

        public ThermApp()
        {
            // setup our global hardware
            ConfigurePeripherals();

            climateController = new ClimateController();
            uxController = new UXController();

            // wire things up
            WireUpEventing();

            // get things spun up
            var t = Start(); //no need to block here .... 
        }

        protected void ConfigurePeripherals()
        {
            temperatureSensor = new AnalogTemperature(
                IOMap.AnalogTempSensor.Device, 
                IOMap.AnalogTempSensor.Pin,
                AnalogTemperature.KnownSensorType.TMP35);
        }

        /// <summary>
        /// Glues things together with the subscribers
        /// </summary>
        protected void WireUpEventing()
        {
            // subscribe to 1/4º C changes in temp
            temperatureSensor.Subscribe(new FilterableChangeObserver<AtmosphericConditionChangeResult, AtmosphericConditions>(
                h => {
                    // probably update screen or something
                    Console.WriteLine($"Update from temp sensor: {h.New.Temperature}ºC");
                    ModelManager.UpdateAmbientTemp(h.New.Temperature.Value);
                },
                e => {
                    return Math.Abs(e.Delta.Temperature.Value) > 0.25f; 
                }));
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
            // BUGBUG: this doesn't seem to be returning
            var conditions = await temperatureSensor.Read();

            // what's weird here is that the screen will update before i see
            // the output of this in the meadow window.
            // not sure why. we're 'wait()ing' this `Start` method, but that
            // shouldn't prevent this writeline from occuring
            Console.WriteLine($"Initial temp: {conditions.Temperature}");

            // it's more reliable if we actually just set a literal here.
            ModelManager.UpdateAmbientTemp(conditions.Temperature.Value);
            //ModelManager.UpdateAmbientTemp(20f);

            //
            Console.WriteLine("Starting up the temp sensor.");
            temperatureSensor.StartUpdating(standbyDuration: 1000);

            Console.WriteLine("Temp sensor spinning");
        }
    }
}
