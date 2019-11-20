using System;
using System.Threading.Tasks;
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
        protected ClimateModel _climate;

        // internals
        protected ISpiBus _spiBus;
        protected ST7789 _display;
        protected GraphicsLibrary _graphics;

        // rending state and lock
        protected bool _rendering = false;
        protected object _renderLock = new object();

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
            _spiBus = ThermApp.Device.CreateSpiBus(
                IOMap.DisplaySpiClock.Item2,
                IOMap.DisplayMosi.Item2,
                IOMap.DisplayMiso.Item2,
                spiConfig);

            // new up the actual display on the SPI bus
            _display = new ST7789(
                device: ThermApp.Device,
                spiBus: _spiBus,
                chipSelectPin: null,
                dcPin: IOMap.DisplayDCPin.Item2,
                resetPin: IOMap.DisplayResetPin.Item2,
                width: 240, height: 240);

            // create our graphics surface that we'll draw onto and then blit
            // to the display with.
            _graphics = new GraphicsLibrary(_display);

            // my display is upside down
            _graphics.CurrentRotation = GraphicsLibrary.Rotation._180Degrees;

            Console.WriteLine("Clear display");

            // finally, clear the display so it's ready for action
            _graphics.Clear(true);
        }

        /// <summary>
        /// Does a display update, if appropriate based on the new climate model
        /// passed in.
        /// </summary>
        /// <param name="model">The climate model that shuld be reflected on
        /// screen.</param>
        public void UpdateClimate(ClimateModel model)
        {
            this._climate = model;
            // update the screen
            this.Render();
        }

        /// <summary>
        /// Does the actual rendering. If it's already rendering, it'll bail out,
        /// so render requests don't stack up.
        /// </summary>
        protected void Render()
        {
            lock (this._renderLock) {
                // if we're already rendering, bail out.
                if (this._rendering) {
                    Console.WriteLine("Already in a rendering loop, bailing out.");
                    return;
                }

                this._rendering = true;
            }

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
