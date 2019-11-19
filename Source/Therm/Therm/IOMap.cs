using System;
using Meadow.Hardware;

namespace Therm
{
    public static class IOMap
    {
        // Display
        public static Tuple<IIODevice, IPin> DisplaySpiClock = new Tuple<IIODevice, IPin>(
            ThermApp.Device, ThermApp.Device.Pins.SCK);
        public static Tuple<IIODevice, IPin> DisplayMosi = new Tuple<IIODevice, IPin>(
            ThermApp.Device, ThermApp.Device.Pins.MOSI);
        public static Tuple<IIODevice, IPin> DisplayMiso = new Tuple<IIODevice, IPin>(
            ThermApp.Device, ThermApp.Device.Pins.MISO);
        public static Tuple<IIODevice, IPin> DisplayDCPin = new Tuple<IIODevice, IPin>(
            ThermApp.Device, ThermApp.Device.Pins.D01);
        public static Tuple<IIODevice, IPin> DisplayResetPin = new Tuple<IIODevice, IPin>(
            ThermApp.Device, ThermApp.Device.Pins.D00);

        // temp sensor
        public static Tuple<IIODevice, IPin> AnalogTempSensor = new Tuple<IIODevice, IPin>(
            ThermApp.Device, ThermApp.Device.Pins.A00);


        // HVAC Control
        public static Tuple<IIODevice, IPin> Heater = new Tuple<IIODevice, IPin>(
            ThermApp.Device, ThermApp.Device.Pins.D10);
        public static Tuple<IIODevice, IPin> AirCon = new Tuple<IIODevice, IPin>(
            ThermApp.Device, ThermApp.Device.Pins.D11);
        public static Tuple<IIODevice, IPin> Fan = new Tuple<IIODevice, IPin>(
            ThermApp.Device, ThermApp.Device.Pins.D12);


        // Buttons
        public static Tuple<IIODevice, IPin> UpButton = new Tuple<IIODevice, IPin>(
            ThermApp.Device, ThermApp.Device.Pins.D03);
        public static Tuple<IIODevice, IPin> DownButton = new Tuple<IIODevice, IPin>(
            ThermApp.Device, ThermApp.Device.Pins.D04);
        public static Tuple<IIODevice, IPin> ModeButton = new Tuple<IIODevice, IPin>(
            ThermApp.Device, ThermApp.Device.Pins.D05);

        static IOMap()
        {
        }
    }
}
