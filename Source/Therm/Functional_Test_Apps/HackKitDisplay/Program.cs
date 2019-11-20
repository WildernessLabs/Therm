using Meadow;
using System.Threading;

namespace HackKitDisplay
{
    class Program
    {
        static IApp app;
        public static void Main(string[] args)
        {
            // instantiate and run new meadow app
            app = new HackKitDisplay();

            Thread.Sleep(Timeout.Infinite);
        }
    }
}
