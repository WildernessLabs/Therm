using System;
using Meadow;
using Meadow.Foundation.Sensors.Buttons;
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
            InitControllers();
            InitializePeripherals();
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
                    _displayController.UpdateClimate(ThermApp.ModelManager.Climate);
                }
            ));

            Console.WriteLine("UXController Up");
        }

        protected void InitializePeripherals()
        {
            Console.WriteLine("UXController.InitializePeripherals");

            _upButton = new PushButton(IOMap.Buttons.Device, IOMap.Buttons.UpPin);
            _downButton = new PushButton(IOMap.Buttons.Device, IOMap.Buttons.DownPin);
            _modeButton = new PushButton(IOMap.Buttons.Device, IOMap.Buttons.ModePin);

            _upButton.Clicked += (s,e) => {
                // TODO: do some checks here:
                //if(this._climateModel.DesiredTemperature + 1 < someMax) {
                Console.WriteLine("Up button");
                ClimateModel newClimate = ClimateModel.From(ThermApp.ModelManager.Climate);
                newClimate.DesiredTemperature++;
                ThermApp.ModelManager.UpdateDesiredClimate(newClimate);
            };

            _downButton.Clicked += (s,e) => {
                //if(this._climateModel.DesiredTemperature - 1 > someMin) {
                Console.WriteLine("Down button");
                ClimateModel newClimate = ClimateModel.From(ThermApp.ModelManager.Climate);
                newClimate.DesiredTemperature--;
                ThermApp.ModelManager.UpdateDesiredClimate(newClimate);
            };

            _modeButton.Clicked += (s, e) => {
                Console.WriteLine("Mode button");
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
    }
}