using System.Threading;
using Meadow;

namespace BasicAnalog_Temp_Sensor
{
    class Program
    {
        static IApp app;
        public static void Main(string[] args)
        {
            // instantiate and run new meadow app
            app = new App();

            Thread.Sleep(Timeout.Infinite);
        }
    }
}
