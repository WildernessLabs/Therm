using System;
using System.Threading;
using Meadow;
using Meadow.Devices;
using Meadow.Foundation.Sensors.Temperature;
using System.Threading.Tasks;
using Meadow.Peripherals.Sensors.Atmospheric;

namespace BasicAnalog_Temp_Sensor
{
    public class App : App<F7Micro, App>
    {
        AnalogTemperature temperatureSensor;

        public App()
        {
            ConfigurePorts();

            //BeginReadingTemp();

            ReadInitialTemperature().Wait();

            // start the temp sensor update loop
            temperatureSensor.StartUpdating();
        }

        public void ConfigurePorts()
        {
            temperatureSensor = new AnalogTemperature(
                Device, Device.Pins.A00, AnalogTemperature.KnownSensorType.LM35
                );

            // subscribe to 1/4º C changes in temp
            temperatureSensor.Subscribe(new FilterableChangeObserver<AtmosphericConditionChangeResult, AtmosphericConditions>(
                h => {
                    // probably update screen or something
                    Console.WriteLine($"Current Temp: {h.New.Temperature}ºC");
                }/*,
                e => { return (Math.Abs(e.Delta.Temperature) > 0.25f); }*/));
        }

        protected async Task ReadInitialTemperature()
        {
            var conditions = await temperatureSensor.Read();
            Console.WriteLine($"Temp: {conditions.Temperature}ºC, {conditions.Temperature.Value.CelciusToFahrenheit()}ºF.");
        }

        protected async Task BeginReadingTemp()
        {
            while (true) {
                // most basic of tests: Annie are you OK, are you ok Annie?
                //Console.WriteLine($"Analog voltage value: {await _annieInnie.Read()}");
                var conditions = await temperatureSensor.Read();
                Console.WriteLine($"Temp: {conditions.Temperature.Value}ºC, {conditions.Temperature.Value.CelciusToFahrenheit()}ºF.");
                Thread.Sleep(1000);
            }
        }
    }

    public static class TemperatureExtension
    {
        public static float CelciusToFahrenheit(this float from)
        {
            return (from * 9) / 5 + 32;
        }
    }
}