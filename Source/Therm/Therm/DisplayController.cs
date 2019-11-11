using System;
using System.Threading.Tasks;

namespace Therm
{
    public class DisplayController
    {
        // TODO: when touch screen is up, raise this when input is received
        public event EventHandler<ClimateModelResult> ClimateModelChanged = delegate { };

        protected ClimateModel _climate;

        public DisplayController()
        {
        }

        protected void InitializePeripherals()
        {
            // TODO: init screen and such
            // use IOMap for pins
        }

        public void UpdateClimate(ClimateModel model)
        {
            this._climate = model;
            // update the screen
            this.Render();
        }

        protected async Task Render()
        {
            await Task.Run(() => {
                //rendering tasks on BG thread
            });
        }
    }
}
