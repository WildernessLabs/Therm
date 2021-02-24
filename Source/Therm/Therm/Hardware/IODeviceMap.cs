using Meadow.Hardware;

namespace Therm
{
    public static class IODeviceMap
    {
        // Display
        public static (IIODevice IODevice,
                        IPin ClockPin,
                        IPin MosiPin,
                        IPin MisoPin,
                        IPin DCPin,
                        IPin ResetPin) Display = (
            ThermApp.Device,
            ThermApp.Device.Pins.SCK,
            ThermApp.Device.Pins.MOSI,
            ThermApp.Device.Pins.MISO,
            ThermApp.Device.Pins.D00,
            ThermApp.Device.Pins.D02);

        // temp sensor
        public static (IIODevice Device, IPin Pin) AnalogTempSensor = (
            ThermApp.Device, ThermApp.Device.Pins.A02);

        // HVAC Control
        public static (IIODevice Device,
                        IPin HeaterPin,
                        IPin AirConPin,
                        IPin FanPin) HVac = (
            ThermApp.Device,
            ThermApp.Device.Pins.D09,
            ThermApp.Device.Pins.D06,
            ThermApp.Device.Pins.D04);

        // Buttons
        public static (IIODevice Device,
                        IPin UpPin,
                        IPin DownPin,
                        IPin ModePin) Buttons = (
            ThermApp.Device,
            ThermApp.Device.Pins.D11,
            ThermApp.Device.Pins.D12,
            ThermApp.Device.Pins.D13);

        static IODeviceMap()
        {
        }
    }
}