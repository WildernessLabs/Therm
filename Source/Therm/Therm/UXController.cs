using System;
using Meadow;
using Meadow.Foundation.Sensors.Buttons;
using Meadow.Hardware;
using Meadow.Peripherals.Sensors.Buttons;

namespace Therm
{
    /// <summary>
    /// In charge of listening for input via the UX and letting the DisplayController
    /// know when to update.
    /// </summary>
    public class UXController
    {
        protected DisplayController _displayController;

        protected IButton _upButton;
        protected IButton _downButton;
        protected IButton _modeButton;

        public UXController()
        {
            this.InitControllers();
            this.InitializePeripherals();
        }

        protected void InitControllers()
        {
            _displayController = new DisplayController();

            // when there's an update from the touch screen, send it on to the
            // model manager (future functionality)
            _displayController.ClimateModelChanged += (s, e) => {
                ThermApp.ModelManager.UpdateDesiredClimate(e.Model);
            };

            // when the climate model changes, make sure to update the UX
            ThermApp.ModelManager.Subscribe(new FilterableObserver<ClimateModelChangeResult, ClimateModel>(
                h => {
                    Console.WriteLine("UXController: Climate model changed, updating display.");
                    this._displayController.UpdateClimate(ThermApp.ModelManager.Climate);
                }
            ));

            Console.WriteLine("UXController Up");
        }

        protected void InitializePeripherals()
        {
            Console.WriteLine("UXController.InitializePeripherals");

            _upButton = new PushButton(IOMap.UpButton.Item1, IOMap.UpButton.Item2);
            _downButton = new PushButton(IOMap.DownButton.Item1, IOMap.DownButton.Item2);
            _modeButton = new PushButton(IOMap.ModeButton.Item1, IOMap.ModeButton.Item2);

            _upButton.Clicked += (s,e) => {
                // TODO: do some checks here:
                //if(this._climateModel.DesiredTemperature + 1 < someMax) {
                ClimateModel newClimate = ClimateModel.From(ThermApp.ModelManager.Climate);
                newClimate.DesiredTemperature++;
                ThermApp.ModelManager.UpdateDesiredClimate(newClimate);
            };

            _upButton.Clicked += (s,e) => {
                //if(this._climateModel.DesiredTemperature - 1 > someMin) {                
                ClimateModel newClimate = ClimateModel.From(ThermApp.ModelManager.Climate);
                newClimate.DesiredTemperature--;
                ThermApp.ModelManager.UpdateDesiredClimate(newClimate);
            };

            _modeButton.Clicked += (s, e) => {
                ClimateModel newClimate = ClimateModel.From(ThermApp.ModelManager.Climate);
                // cycle to the next mode
                switch (newClimate.HvacOperatingMode) {
                    case ClimateController.Mode.Auto:
                        newClimate.HvacOperatingMode = ClimateController.Mode.Cool;
                        break;
                    case ClimateController.Mode.Cool:
                        newClimate.HvacOperatingMode = ClimateController.Mode.Heat;
                        break;
                    case ClimateController.Mode.Heat:
                        newClimate.HvacOperatingMode = ClimateController.Mode.FanOnly;
                        break;
                    case ClimateController.Mode.FanOnly:
                        newClimate.HvacOperatingMode = ClimateController.Mode.Off;
                        break;
                    case ClimateController.Mode.Off:
                        newClimate.HvacOperatingMode = ClimateController.Mode.Auto;
                        break;
                }
                ThermApp.ModelManager.UpdateDesiredClimate(newClimate);
            };
        }

        ///// <summary>
        ///// Updates the UX with the current climate conditions and operating mode.
        ///// </summary>
        ///// <param name="climateModel"></param>
        //public void UpdateUX(ClimateModel climateModel)
        //{
        //    // update the display
        //    this._displayController.UpdateClimate(climateModel);
        //}
    }
}
