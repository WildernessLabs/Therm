using System;
using Meadow.Hardware;

namespace Therm
{
    public static class IOMap
    {
        // HVAC Control
        public static Tuple<IIODevice, IPin> Heater = new Tuple<IIODevice, IPin>(
            ThermApp.Device, ThermApp.Device.Pins.D00);
        public static Tuple<IIODevice, IPin> AirCon = new Tuple<IIODevice, IPin>(
            ThermApp.Device, ThermApp.Device.Pins.D01);
        public static Tuple<IIODevice, IPin> Fan = new Tuple<IIODevice, IPin>(
            ThermApp.Device, ThermApp.Device.Pins.D02);


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
