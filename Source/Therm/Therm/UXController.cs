using System;
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
        /// <summary>
        /// Raised when the desired climate settings are changed via the display
        /// or physical UI.
        /// </summary>
        public event EventHandler<ClimateModelResult> ClimateModelChanged = delegate { };

        protected DisplayController _displayController;

        protected ClimateModel _climateModel;

        protected IButton _upButton;
        protected IButton _downButton;
        protected IButton _modeButton;

        public UXController()
        {
            this.InitializePeripherals();
            this.InitControllers();
        }

        protected void InitControllers()
        {
            _displayController = new DisplayController();
        }

        protected void InitializePeripherals()
        {
            _upButton = new PushButton(IOMap.UpButton.Item1, IOMap.UpButton.Item2);
            _downButton = new PushButton(IOMap.DownButton.Item1, IOMap.DownButton.Item2);
            _modeButton = new PushButton(IOMap.ModeButton.Item1, IOMap.DownButton.Item2);

            _upButton.Clicked += (s,e) => {
                //if(this._climateModel.DesiredTemperature + 1 < someMax) {
                ClimateModel newClimate = ClimateModel.From(this._climateModel);
                newClimate.DesiredTemperature++;
                this.UpdateUX(newClimate);
                this.RaiseClimateModelChange(newClimate);
            };

            _upButton.Clicked += (s,e) => {
                //if(this._climateModel.DesiredTemperature - 1 > someMin) {
                ClimateModel newClimate = ClimateModel.From(this._climateModel);
                newClimate.DesiredTemperature--;
                this.UpdateUX(newClimate);
                this.RaiseClimateModelChange(newClimate);
            };

            _modeButton.Clicked += (s, e) => {
                ClimateModel newClimate = ClimateModel.From(this._climateModel);
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
                this.UpdateUX(newClimate);
                this.RaiseClimateModelChange(newClimate);
            };
        }

        /// <summary>
        /// Updates the UX with the current climate conditions and operating mode.
        /// </summary>
        /// <param name="climateModel"></param>
        public void UpdateUX(ClimateModel climateModel)
        {
            //
            this._climateModel = climateModel;
            // TODO: call into the DisplayController.Update or whatever;
        }

        protected void RaiseClimateModelChange(ClimateModel model)
        {
            this.ClimateModelChanged?.Invoke(this, new ClimateModelResult(model));
        }
    }
}
