using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Displays.Tft;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Sensors.Buttons;
using Meadow.Hardware;
using System;

namespace ThermUX
{
    public class MeadowApp : App<F7Micro, MeadowApp>
    {
        St7789 st7789;
        PushButton buttonUp;
        PushButton buttonDown;
        PushButton buttonMode;
        GraphicsLibrary graphics;

        public MeadowApp()
        {
            buttonUp = new PushButton(Device, Device.Pins.D02);
            buttonUp.Clicked += ButtonUpClicked;
            buttonDown = new PushButton(Device, Device.Pins.D03);
            buttonDown.Clicked += ButtonDownClicked;
            buttonMode = new PushButton(Device, Device.Pins.D04);
            buttonMode.Clicked += ButtonModeClicked;

            var config = new SpiClockConfiguration(6000, SpiClockConfiguration.Mode.Mode3);
            st7789 = new St7789
            (
                device: Device,
                spiBus: Device.CreateSpiBus(Device.Pins.SCK, Device.Pins.MOSI, Device.Pins.MISO, config),
                chipSelectPin: Device.Pins.D02,
                dcPin: Device.Pins.D01,
                resetPin: Device.Pins.D00,
                width: 240, height: 240
            );

            graphics = new GraphicsLibrary(st7789);

            //LoadWelcomeScreen();
            MainScreen();
        }

        void ButtonUpClicked(object sender, EventArgs e)
        {
            Console.WriteLine("Up button clicked!");
        }

        void ButtonDownClicked(object sender, EventArgs e)
        {
            Console.WriteLine("Down button clicked!");
        }

        void ButtonModeClicked(object sender, EventArgs e)
        {
            Console.WriteLine("Mode button clicked!");
        }

        void LoadWelcomeScreen() 
        {
            string thermv01 = "Therm v0.1";

            graphics.Clear(true);

            graphics.DrawRectangle(0, 0, (int)st7789.Width, (int)st7789.Height, Color.White);
            graphics.DrawRectangle(5, 5, (int)st7789.Width - 10, (int)st7789.Height - 10, Color.White);

            graphics.DrawCircle((int)st7789.Width/2, (int)st7789.Height / 2, (int)(st7789.Width / 2) - 10, Color.Red, true);

            graphics.CurrentFont = new Font12x20();
            graphics.DrawText((int)(st7789.Width - thermv01.Length*12) / 2, 110, thermv01, Color.Black);

            graphics.Show();
        }

        void MainScreen() 
        {
            string cooling = "COOLING";
            string thermv01 = "23.4 °C";
            string inTimer = "IN 30 MINUTES";

            graphics.Clear(true);

            graphics.DrawRectangle(0, 0, (int)st7789.Width, (int)st7789.Height, Color.White);
            graphics.DrawRectangle(5, 5, (int)st7789.Width - 10, (int)st7789.Height - 10, Color.White);

            graphics.DrawCircle((int)st7789.Width / 2, (int)st7789.Height / 2, (int)(st7789.Width / 2) - 10, Color.Red, true);

            graphics.CurrentFont = new Font12x20();
            graphics.DrawText((int)(st7789.Width - cooling.Length * 12) / 2,  (int)(st7789.Height * 0.30) - 10, cooling, Color.Blue);
            graphics.DrawText((int)(st7789.Width - thermv01.Length * 12) / 2, (int)(st7789.Height * 0.50) - 10, thermv01, Color.Black);
            graphics.DrawText((int)(st7789.Width - inTimer.Length * 12) / 2,  (int)(st7789.Height * 0.70) - 10, inTimer, Color.Black);

            graphics.Show();
        }
    }
}