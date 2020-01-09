using System;
using Meadow.Hardware;

namespace Therm
{
    public static class IOMap
    {
        // Display
        public static (IIODevice Device, IPin Pin) DisplaySpiClock = (
            ThermApp.Device, ThermApp.Device.Pins.SCK);
        public static (IIODevice Device, IPin Pin) DisplayMosi = (
            ThermApp.Device, ThermApp.Device.Pins.MOSI);
        public static (IIODevice Device, IPin Pin) DisplayMiso = (
            ThermApp.Device, ThermApp.Device.Pins.MISO);
        public static (IIODevice Device, IPin Pin) DisplayDCPin = (
            ThermApp.Device, ThermApp.Device.Pins.D01);
        public static (IIODevice Device, IPin Pin) DisplayResetPin = (
            ThermApp.Device, ThermApp.Device.Pins.D00);

        // temp sensor
        public static (IIODevice Device, IPin Pin) AnalogTempSensor = (
            ThermApp.Device, ThermApp.Device.Pins.A00);


        // HVAC Control
        public static (IIODevice Device, IPin Pin) Heater = (
            ThermApp.Device, ThermApp.Device.Pins.D10);
        public static (IIODevice Device, IPin Pin) AirCon = (
            ThermApp.Device, ThermApp.Device.Pins.D11);
        public static (IIODevice Device, IPin Pin) Fan = (
            ThermApp.Device, ThermApp.Device.Pins.D12);


        // Buttons
        public static (IIODevice Device, IPin Pin) UpButton = (
            ThermApp.Device, ThermApp.Device.Pins.D03);
        public static (IIODevice Device, IPin Pin) DownButton = (
            ThermApp.Device, ThermApp.Device.Pins.D04);
        public static (IIODevice Device, IPin Pin) ModeButton = (
            ThermApp.Device, ThermApp.Device.Pins.D05);

        static IOMap()
        {
        }
    }
}