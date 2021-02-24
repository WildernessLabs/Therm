using System;
using System.Threading;
using System.Threading.Tasks;
using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Leds;
using Meadow.Foundation.Sensors.Atmospheric;
using Meadow.Foundation.Sensors.Temperature;
using Meadow.Hardware;
using Meadow.Foundation.Relays;
using Meadow.Foundation.Displays.Tft;
using Meadow.Foundation.Graphics;

namespace MeadowApp
{
    public class MeadowApp : App<F7Micro, MeadowApp>
    {
        RgbPwmLed onboardLed;
        Bme280 bme280;
        AnalogTemperature anlgTemp;
        //IAnalogInputPort a02;
        St7789 display;
        GraphicsLibrary canvas;
        Relay[] relays = new Relay[5];
        // internals
        int dW = 240; // display width 
        int dH = 240; // display height

        Color[] Colors = new Color[] {
                WildernessLabsColors.AzureBlue,
                WildernessLabsColors.PearGreen,
                WildernessLabsColors.ChileanFire,
                WildernessLabsColors.GalleryWhite };


        public MeadowApp()
        {
            Initialize();

            //
            Console.WriteLine("Reading temp");
            ReadConditions().Wait();
            //ReadAnalogTemp().Wait();

            //Console.WriteLine($"Analog voltage: {(float)a02.Read().Result}");

            FillDisplayColors();

            CycleRelays();

        }

        void Initialize()
        {
            Console.WriteLine("Initialize hardware...");

            Console.WriteLine("Onboard LED");
            onboardLed = new RgbPwmLed(device: Device,
                redPwmPin: Device.Pins.OnboardLedRed,
                greenPwmPin: Device.Pins.OnboardLedGreen,
                bluePwmPin: Device.Pins.OnboardLedBlue,
                3.3f, 3.3f, 3.3f,
                Meadow.Peripherals.Leds.IRgbLed.CommonType.CommonAnode);

            // configure our BME280 on the I2C Bus
            Console.WriteLine("BME280");
            var i2c = Device.CreateI2cBus();
            bme280 = new Bme280(
                i2c,
                Bme280.I2cAddress.Adddress0x76
            );

            // configure our AnalogTemperature sensor
            Console.WriteLine("Analog Temp");
            anlgTemp = new AnalogTemperature(
                device: Device,
                analogPin: Device.Pins.A02,
                sensorType: AnalogTemperature.KnownSensorType.TMP35
            );

            //a02 = Device.CreateAnalogInputPort(Device.Pins.A02);

            Console.WriteLine("Relays");
            relays[0] = new Relay(Device, Device.Pins.D04); // Fan
            relays[1] = new Relay(Device, Device.Pins.D09); // Heat 1
            relays[2] = new Relay(Device, Device.Pins.D10); // Heat 2
            relays[3] = new Relay(Device, Device.Pins.D06); // Cool 1
            relays[4] = new Relay(Device, Device.Pins.D05); // Cool 2

            Console.WriteLine("Display");
            var config = new SpiClockConfiguration(48000, SpiClockConfiguration.Mode.Mode3);
            var spiBus = Device.CreateSpiBus(Device.Pins.SCK, Device.Pins.MOSI, Device.Pins.MISO, config);
            display = new St7789(
                device: Device,
                spiBus: spiBus,
                chipSelectPin: null,
                dcPin: Device.Pins.D00,
                resetPin: Device.Pins.D02,
                width: 240, height: 240);
           canvas = new GraphicsLibrary(display);
        }

        protected async Task ReadConditions()
        {
            var conditions = await bme280.Read();
            Console.WriteLine("Initial Readings:");
            Console.WriteLine($"  Temperature: {conditions.Temperature}°C");
            Console.WriteLine($"  Pressure: {conditions.Pressure}hPa");
            Console.WriteLine($"  Relative Humidity: {conditions.Humidity}%");
        }

        //protected async Task ReadAnalogTemp()
        //{
        //    var conditions = await anlgTemp.Read();
        //    Console.WriteLine($"Initial temp: { conditions.Temperature }");
        //}

        protected void CycleRelays()
        {
            for (int i = 0; i < 10; i++) {

                foreach (var relay in relays) {
                    //turn all off
                    TurnOffAllRelays();
                    relay.IsOn = true;
                    Thread.Sleep(250);
                    relay.IsOn = false;
                }
            }
        }

        protected void TurnOffAllRelays()
        {
            foreach (var relay in relays) {
                relay.IsOn = false;
            }
        }

        protected void DisplaySomeStuff()
        {

        }

        public void FillDisplayColors()
        {
            Console.WriteLine("FillDisplayColors()");

            foreach (var color in Colors) {
                FillWithColor(color);
            }
        }

        // drawing helpers
        void FillWithColor(Color color)
        {
            // clear our buffer
            canvas.Clear();
            // draw a filled rectangle that fills the screen
            canvas.DrawRectangle(0, 0, dW, dH, color, filled: true);
            // copy the canvas contents to the device
            onboardLed.SetColor(color);
            canvas.Show();
        }

    }

    public static class WildernessLabsColors
    {
        public static Color AzureBlue = Color.FromHex("#23abe3");
        public static Color AzureBlueSecondary = Color.FromHex("#5AC0EA");
        public static Color AzureBlueLight = Color.FromHex("#E3F4FB");
        public static Color AzureBlueDark = Color.FromHex("#05161D");

        public static Color PearGreen = Color.FromHex("#C9DB31");
        public static Color PearGreenSecondary = Color.FromHex("#D7E465");
        public static Color PearGreenLight = Color.FromHex("#F8FAE4");
        public static Color PearGreenDark = Color.FromHex("#212319");

        public static Color MetallicBronze = Color.FromHex("#524740");
        public static Color MetallicBronzeSecondary = Color.FromHex("#7D7570");
        public static Color MetallicBronzeLight = Color.FromHex("#E9E7E6");
        public static Color MetallicBronzeDark = Color.FromHex("#0B0908");

        public static Color ChileanFire = Color.FromHex("#EF7D3B");
        public static Color ChileanFireSecondary = Color.FromHex("#F39E6C");
        public static Color ChileanFireLight = Color.FromHex("#FDEEE6");
        public static Color ChileanFireDark = Color.FromHex("#1F1008");

        public static Color GalleryWhite = Color.FromHex("#EEEEEE");
        public static Color GalleryWhiteSecondary = Color.FromHex("#F2F2F2");
        public static Color GalleryWhiteLight = Color.FromHex("#FDFDFD");
        public static Color GalleryWhiteDark = Color.FromHex("#1F1F1F");

        public static Color DustyGray = Color.FromHex("#999999");
        public static Color DustyGraySecondary = Color.FromHex("#B3B3B3");
        public static Color DustyGrayLight = Color.FromHex("#F2F2F2");
        public static Color DustyGrayDark = Color.FromHex("#141414");

        public static Color Sandrift = Color.FromHex("#B09679");
        public static Color SandriftSecondary = Color.FromHex("#C4B09B");
        public static Color SandriftLight = Color.FromHex("#F5F1EE");
        public static Color SandriftDark = Color.FromHex("#171310");

    }

}