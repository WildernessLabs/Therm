using System;
using System.Threading.Tasks;
using Meadow.Foundation;
using Meadow.Foundation.Displays.Tft;
using Meadow.Foundation.Graphics;
using Meadow.Hardware;

namespace Therm
{
    public class DisplayController
    {
        // TODO: when touch screen is up, raise this when input is received
        public event EventHandler<ClimateModelResult> ClimateModelChanged = delegate { };

        protected ClimateModel _climate;

        protected ISpiBus _spiBus;
        protected ST7789 _display;
        protected GraphicsLibrary _graphics;

        protected bool _rendering = false;
        protected object _renderLock = new object();

        public DisplayController()
        {
            InitializeDisplay();
        }

        protected void InitializeDisplay()
        {
            // our display needs mode3
            var spiConfig = new SpiClockConfiguration(
                6000,
                SpiClockConfiguration.Mode.Mode3);

            _spiBus = ThermApp.Device.CreateSpiBus(
                IOMap.DisplaySpiClock.Item2,
                IOMap.DisplayMosi.Item2,
                IOMap.DisplayMiso.Item2,
                spiConfig);

            _display = new ST7789(
                device: ThermApp.Device,
                spiBus: _spiBus,
                chipSelectPin: null,
                dcPin: IOMap.DisplayDCPin.Item2,
                resetPin: IOMap.DisplayResetPin.Item2,
                width: 240, height: 240);

            _graphics = new GraphicsLibrary(_display);

            // my display is upside down
            _graphics.CurrentRotation = GraphicsLibrary.Rotation._180Degrees;

            Console.WriteLine("Clear display");

            _graphics.Clear(true);
        }

        public void UpdateClimate(ClimateModel model)
        {
            this._climate = model;
            // update the screen
            this.Render();
        }

        protected void Render()
        {
            lock (this._renderLock) {
                // if we're already rendering, bail out.
                if (this._rendering) {
                    Console.WriteLine("Already in a rendering loop, bailing out.");
                    return;
                }
            }

            this._rendering = true;

                //            Task.Run(() => {
                //rendering tasks on BG thread
                _graphics.Clear();
                _graphics.CurrentFont = new Font12x16();
                _graphics.DrawText(4, 4, $"current temp: {_climate.CurrentConditions.Temperature.ToString("###")}º", Color.FromHex("24abe3"));
                _graphics.DrawText(4, 20, $"desired temp: {_climate.DesiredTemperature.ToString("###")}º", Color.FromHex("EF7D3B"));
                _graphics.DrawText(4, 40, "all temps Canadian", Color.White);
                _graphics.Show();
            //            });

            this._rendering = false;
            
        }
    }
}
