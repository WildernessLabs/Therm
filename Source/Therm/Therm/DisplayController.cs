using System;
namespace Therm
{
    public class DisplayController
    {
        // TODO: when touch screen is up, add a climate changed event

        protected ClimateModel _desiredClimate;

        public DisplayController()
        {
        }

        protected void InitializePeripherals()
        {
            // TODO: init screen and such
            // use IOMap for pins
        }

        public void UpdateDisplay(ClimateModel model)
        {
            this._desiredClimate = model;
            // TODO: Do stuff.
        }
    }
}
