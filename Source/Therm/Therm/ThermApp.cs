using System;
using Meadow;
using Meadow.Devices;
using Meadow.Foundation.Sensors.Temperature;
using Meadow.Peripherals.Sensors.Atmospheric;
using System.Threading.Tasks;
using Meadow.Foundation.Web.Maple.Server;
using Meadow.Gateway.WiFi;

namespace Therm
{
    public class ThermApp : App<F7Micro, ThermApp>
    {
        // other
        MapleServer mapleServer;
        AmbientConditionsListener ambientConditionsListener;
        ClimateController climateController;
        HvacController hvacController;
        UXController uxController;

        public static ClimateModelManager ModelManager { get => ClimateModelManager.Instance; }

        public ThermApp()
        {
            Initialize().Wait();

            // get things spun up
            _ = Start(); //no need to block here .... 
        }

        async Task Initialize()
        {
            Console.WriteLine("Initialize hardware...");

            ambientConditionsListener = new AmbientConditionsListener();
            climateController = new ClimateController();
            uxController = new UXController();
            Console.WriteLine("Controllers up.");

            // initialize the wifi adpater
            if (!Device.InitWiFiAdapter().Result) {
                throw new Exception("Could not initialize the WiFi adapter.");
            }

            // connnect to the wifi network.
            Console.WriteLine($"Connecting to WiFi Network {Secrets.WIFI_NAME}");
            var connectionResult = await Device.WiFiAdapter.Connect(Secrets.WIFI_NAME, Secrets.WIFI_PASSWORD);
            if (connectionResult.ConnectionStatus != ConnectionStatus.Success) {
                throw new Exception($"Cannot connect to network: {connectionResult.ConnectionStatus}");
            }
            Console.WriteLine($"Connected. IP: {Device.WiFiAdapter.IpAddress}");

            // create our maple web server
            mapleServer = new MapleServer(
                Device.WiFiAdapter.IpAddress
                );

            Console.WriteLine("Initialization complete.");
        }



        /// <summary>
        /// Kicks off the app. Starts by doing a temp read and then spins
        /// up the sensor updating and such.
        /// </summary>
        /// <returns></returns>
        protected async Task Start()
        {
            // take an initial reading of the temp
            Console.WriteLine("Start");

            // start our web server
            mapleServer.Start();

        }
    }
}