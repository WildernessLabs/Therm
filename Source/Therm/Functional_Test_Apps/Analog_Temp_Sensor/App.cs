using System;
using System.Threading;
using Meadow;
using Meadow.Devices;
using Meadow.Hardware;
using Meadow.Foundation.Sensors.Temperature;

namespace BasicAnalog_Temp_Sensor
{
    public class App : App<F7Micro, App>
    {
        AnalogTemperature _tmpSensor;
        //IAnalogInputPort _annieInnie;

        public App()
        {
            ConfigurePorts();
            BeginReadingTemp();
        }

        public void ConfigurePorts()
        {
            
            //_annieInnie = Device.CreateAnalogInputPort(Device.Pins.A00);

            _tmpSensor = new AnalogTemperature(
                Device, Device.Pins.A00, AnalogTemperature.KnownSensorType.LM35
                );


        }

        protected async void BeginReadingTemp()
        {
            while (true) {
                // most basic of tests: Annie are you OK, are you ok Annie?
                //Console.WriteLine($"Analog voltage value: {await _annieInnie.Read()}");


                var temp = await _tmpSensor.Read();
                Console.WriteLine("Temp: " + temp);
                Thread.Sleep(1000);
            }
        }

    }
}
