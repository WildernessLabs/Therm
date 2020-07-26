using System;
using System.Threading;
using Meadow;
using Meadow.Devices;
using Meadow.Hardware;
using Meadow.Foundation.Sensors.Temperature;
using System.Threading.Tasks;
using Meadow.Peripherals.Sensors.Atmospheric;

namespace BasicAnalog_Temp_Sensor
{
    public class App : App<F7Micro, App>
    {
        AnalogTemperature _tmpSensor;
        //IAnalogInputPort _annieInnie;

        public App()
        {
            ConfigurePorts();

            //BeginReadingTemp();

            this.ReadInitialTemp().Wait();

            // start the temp sensor update loop
            this._tmpSensor.StartUpdating();
        }

        public void ConfigurePorts()
        {

            //_annieInnie = Device.CreateAnalogInputPort(Device.Pins.A00);

            _tmpSensor = new AnalogTemperature(
                Device, Device.Pins.A00, AnalogTemperature.KnownSensorType.LM35
                );

            // subscribe to 1/4º C changes in temp
            this._tmpSensor.Subscribe(new FilterableChangeObserver<AtmosphericConditionChangeResult, AtmosphericConditions>(
                h => {
                    // probably update screen or something
                    Console.WriteLine($"Current Temp: {h.New.Temperature}ºC");
                }/*,
                e => { return (Math.Abs(e.Delta.Temperature) > 0.25f); }*/));
            
        }

        protected async Task ReadInitialTemp()
        {
            var conditions = await _tmpSensor.Read();
            Console.WriteLine($"Temp: {conditions.Temperature}ºC, {conditions.Temperature.Value.ToFahrenheit()}ºF.");
        }

        protected async Task BeginReadingTemp()
        {
            while (true) {
                // most basic of tests: Annie are you OK, are you ok Annie?
                //Console.WriteLine($"Analog voltage value: {await _annieInnie.Read()}");

                var conditions = await _tmpSensor.Read();
                Console.WriteLine($"Temp: {conditions.Temperature.Value}ºC, {conditions.Temperature.Value.ToFahrenheit()}ºF.");
                Thread.Sleep(1000);
            }
        }

    }
    public static class TempExtension
    {
        public static float ToFahrenheit(this float from)
        {
            return (from * 9) / 5 + 32;
        }
    }
}
