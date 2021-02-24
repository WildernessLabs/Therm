using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Displays.Tft;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Sensors.Buttons;
using Meadow.Hardware;
using System;
using System.Threading;

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
            Console.WriteLine("Start...");
            
            //buttonUp = new PushButton(Device, Device.Pins.D02);
            //buttonUp.Clicked += ButtonUpClicked;
            
            //buttonDown = new PushButton(Device, Device.Pins.D03);
            //buttonDown.Clicked += ButtonDownClicked;
            
            //buttonMode = new PushButton(Device, Device.Pins.D04);
            //buttonMode.Clicked += ButtonModeClicked;

            var config = new SpiClockConfiguration(48000, SpiClockConfiguration.Mode.Mode3);
            st7789 = new St7789
            (
                device: Device,
                spiBus: Device.CreateSpiBus(Device.Pins.SCK, Device.Pins.MOSI, Device.Pins.MISO, config),
                chipSelectPin: null,
                dcPin: Device.Pins.D00,
                resetPin: Device.Pins.D02,
                width: 240, height: 240
            );

            graphics = new GraphicsLibrary(st7789);

            //WelcomeScreen();
            //MainScreen();
            AdjustTemperatureScreen();
            Console.WriteLine("End...");
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

        void WelcomeScreen() 
        {
            string thermv01 = "Therm v0.1";

            graphics.Clear(true);

            graphics.Stroke = 1;
            graphics.DrawRectangle(0, 0, (int)st7789.Width, (int)st7789.Height, Color.White);
            graphics.DrawRectangle(5, 5, (int)st7789.Width - 10, (int)st7789.Height - 10, Color.White);

            graphics.DrawCircle((int)st7789.Width/2, (int)st7789.Height / 2, (int)(st7789.Width / 2) - 10, Color.White, true);

            graphics.CurrentFont = new Font12x20();
            graphics.DrawText((int)(st7789.Width - thermv01.Length*12) / 2, 110, thermv01, Color.Black);

            graphics.Show();
        }

        void MainScreen() 
        {
            Console.WriteLine("MainScreen()...");

            string cooling = "COOLING";
            string thermv01 = "23.4 °C";
            string inTimer = "IN 30 MINUTES";

            graphics.Clear(true);

            graphics.Stroke = 1;
            graphics.DrawRectangle(0, 0, (int)st7789.Width, (int)st7789.Height, Color.White);
            graphics.DrawRectangle(5, 5, (int)st7789.Width - 10, (int)st7789.Height - 10, Color.White);

            graphics.DrawCircle((int)st7789.Width / 2, (int)st7789.Height / 2, (int)(st7789.Width / 2) - 10, Color.White, true);

            graphics.CurrentFont = new Font12x20(); 
            graphics.DrawText((int)(st7789.Width - cooling.Length * 12) / 2,  (int)(st7789.Height * 0.30) - 10, cooling, Color.Blue);
            graphics.DrawText((int)(st7789.Width - thermv01.Length * 24) / 2, (int)(st7789.Height * 0.50) - 20, thermv01, Color.Black, GraphicsLibrary.ScaleFactor.X2);
            graphics.DrawText((int)(st7789.Width - inTimer.Length * 12) / 2,  (int)(st7789.Height * 0.70) - 10, inTimer, Color.Black);

            graphics.Show();
        }

        void AdjustTemperatureScreen() 
        {
            Console.WriteLine("MainScreen()...");

            float temperature = 23.5f;

            graphics.Stroke = 1;
            graphics.DrawRectangle(0, 0, (int)st7789.Width, (int)st7789.Height, Color.White);
            graphics.DrawRectangle(5, 5, (int)st7789.Width - 10, (int)st7789.Height - 10, Color.White);
            graphics.Show();

            Console.WriteLine("1...");

            graphics.DrawCircle((int)st7789.Width / 2, (int)st7789.Height / 2, (int)(st7789.Width / 2) - 10, Color.White, true);
            graphics.Show();

            Console.WriteLine("2...");

            graphics.DrawTriangle(
                x0: (int)(st7789.Width * 0.35),
                y0: (int)(st7789.Height * 0.30),
                x1: (int)(st7789.Width * 0.5),
                y1: (int)(st7789.Height * 0.15),
                x2: (int)(st7789.Width * 0.65),
                y2: (int)(st7789.Height * 0.30),
                color: Color.Black,
                filled: true);

            graphics.DrawTriangle(
                x0: (int)(st7789.Width * 0.35),
                y0: (int)(st7789.Height * 0.70),
                x1: (int)(st7789.Width * 0.5),
                y1: (int)(st7789.Height * 0.85),
                x2: (int)(st7789.Width * 0.65),
                y2: (int)(st7789.Height * 0.70),
                color: Color.Black,
                filled: true);
            graphics.Show();

            graphics.CurrentFont = new Font12x20();
            graphics.DrawText(
                (int)(st7789.Width - temperature.ToString().Length * 24) / 2,
                (int)(st7789.Height * 0.50) - 20,
                temperature.ToString("##.##"), Color.Black, GraphicsLibrary.ScaleFactor.X2);
            graphics.Show();

            Console.WriteLine("3...");

            for (int i = 1; i < 10; i++)
            {
                graphics.DrawText(
                    (int)(st7789.Width - temperature.ToString().Length * 24) / 2,
                    (int)(st7789.Height * 0.50) - 20,
                    temperature.ToString("##.##"), Color.White, GraphicsLibrary.ScaleFactor.X2);

                Console.WriteLine($"4... {temperature}");
                temperature++;

                graphics.DrawText(
                    (int)(st7789.Width - temperature.ToString().Length * 24) / 2,
                    (int)(st7789.Height * 0.50) - 20,
                    temperature.ToString("##.##"), Color.Black, GraphicsLibrary.ScaleFactor.X2);
                graphics.Show();

                Thread.Sleep(1000);
            }
        }
    }
}