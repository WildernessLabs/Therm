using Meadow;
using System.Threading;

namespace Therm
{
    class Program
    {
        static IApp app;
        public static void Main(string[] args)
        {
            // instantiate and run new meadow app
            app = new ThermApp();

            Thread.Sleep(Timeout.Infinite);
        }
    }
}