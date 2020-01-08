using System;
using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Displays.Tft;
using Meadow.Hardware;
using Meadow.Foundation.Graphics;

namespace HackKitDisplay
{
    public class HackKitDisplay : App<F7Micro, HackKitDisplay>
    {
        protected ISpiBus _spiBus;
        protected St7789 _display;
        GraphicsLibrary _graphics;

        public HackKitDisplay()
        {
            var config = new SpiClockConfiguration(6000, SpiClockConfiguration.Mode.Mode3);
            _spiBus = Device.CreateSpiBus(Device.Pins.SCK, Device.Pins.MOSI, Device.Pins.MISO, config);

            _display = new St7789(
                device: Device,
                spiBus: _spiBus,
                chipSelectPin: null,
                dcPin: Device.Pins.D01,
                resetPin: Device.Pins.D00,
                width: 240, height: 240);

            _graphics = new GraphicsLibrary(_display);

            // my display is upside down"s
            _graphics.Rotation = GraphicsLibrary.RotationType._180Degrees;

            Console.WriteLine("Clear display");
            //_display.ClearScreen(250);
            //_display.Refresh();

            _graphics.Clear(true);

            //Console.WriteLine("Draw lines");
            //for (int i = 0; i < 30; i++) {
            //    _display.DrawPixel(i, i, true);
            //    _display.DrawPixel(30 + i, i, true);
            //    _display.DrawPixel(60 + i, i, true);
            //}

            _graphics.CurrentFont = new Font12x16();
            _graphics.DrawText(4, 4, "current temp: 26º", Color.FromHex("24abe3"));
            _graphics.DrawText(4, 20, "desired temp: 24º", Color.FromHex("EF7D3B"));
            _graphics.DrawText(4, 40, "all temps Canadian", Color.White);
            _graphics.Show();

            //Console.WriteLine("Show()");
            //_display.Show();
            //Console.WriteLine("shown.");
        }
    }
}