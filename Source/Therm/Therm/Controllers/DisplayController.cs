﻿using System;
using Meadow.Foundation;
using Meadow.Foundation.Displays.Tft;
using Meadow.Foundation.Graphics;
using Meadow.Hardware;

namespace Therm
{
    /// <summary>
    /// This controller is in charge of the physical display, including rendering.
    ///
    /// It's also wired up to handle input on the display in the future that may
    /// change the desired climate.
    /// </summary>
    public class DisplayController
    {
        // TODO: when touch screen is up, raise this when input is received
        // TODO: consider actually coupling with the ClimateModelManager, the
        // way the rest of the app works. might simplify things. might now.
        public event EventHandler<ClimateModelResult> ClimateModelChanged = delegate { };

        /// <summary>
        /// local climate state.
        /// </summary>
        protected ClimateModel climate;

        // internals
        protected St7789 display;
        protected GraphicsLibrary graphics;

        // rending state and lock
        protected bool isRendering = false;
        protected object renderLock = new object();

        public DisplayController()
        {
            InitializeDisplay();
        }

        /// <summary>
        /// intializes the physical display peripheral, as well as the backing
        /// graphics library.
        /// </summary>
        protected void InitializeDisplay()
        {
            // our display needs mode3
            var spiConfig = new SpiClockConfiguration(
                6000,
                SpiClockConfiguration.Mode.Mode3);

            // initialize our SPI bus, with that config
            var spiBus = ThermApp.Device.CreateSpiBus(
                IODeviceMap.Display.ClockPin,
                IODeviceMap.Display.MosiPin,
                IODeviceMap.Display.MisoPin,
                spiConfig);

            // new up the actual display on the SPI bus
            display = new St7789(
                device: ThermApp.Device,
                spiBus: spiBus,
                chipSelectPin: null,
                dcPin: IODeviceMap.Display.DCPin,
                resetPin: IODeviceMap.Display.ResetPin,
                width: 240, height: 240);

            // create our graphics surface that we'll draw onto and then blit
            // to the display with.
            graphics = new GraphicsLibrary(display)
            {   // my display is upside down
                // Rotation = GraphicsLibrary.RotationType._180Degrees,
                CurrentFont = new Font12x20(),
            };

            Console.WriteLine("Clear display");

            // finally, clear the display so it's ready for action
            graphics.Clear(true);

            Render();
        }

        /// <summary>
        /// Does a display update, if appropriate based on the new climate model
        /// passed in.
        /// </summary>
        /// <param name="model">The climate model that shuld be reflected on
        /// screen.</param>
        public void UpdateClimate(ClimateModel model)
        {
            climate = model;
            // update the screen
            Render();
        }

        /// <summary>
        /// Does the actual rendering. If it's already rendering, it'll bail out,
        /// so render requests don't stack up.
        /// </summary>
        protected void Render()
        {
            Console.WriteLine($"Render() - is rendering: {isRendering}");

            lock (renderLock)
            {   // if we're already rendering, bail out.
                if (isRendering)
                {
                    Console.WriteLine("Already in a rendering loop, bailing out.");
                    return;
                }

                isRendering = true;
            }

            //            Task.Run(() => {
            //rendering tasks on BG thread
            Console.WriteLine("Clear");

            graphics.Clear();

            Console.WriteLine("DrawText");
            if(climate != null && climate.CurrentConditions != null)
            {
                graphics.DrawText(4, 4, $"current temp: {climate.CurrentConditions.Temperature.Value.ToString("###")}°", Color.FromHex("24abe3"));
                graphics.DrawText(4, 20, $"desired temp: {climate.DesiredTemperature.ToString("###")}°", Color.FromHex("EF7D3B"));

            }
            graphics.DrawText(4, 40, "all temps Canadian", Color.White);
            Console.WriteLine("Show");
            graphics.Show();
            //            });

            Console.WriteLine("Show complete");

            isRendering = false;
            
        }
    }
}
