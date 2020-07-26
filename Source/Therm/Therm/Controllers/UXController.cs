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
        protected DisplayController displayController;

        protected IButton upButton;
        protected IButton downButton;
        protected IButton modeButton;

        public UXController()
        {
            InitControllers();
            InitializePeripherals();
        }

        protected void InitControllers()
        {
            displayController = new DisplayController();

            // when there's an update from the touch screen, send it on to the
            // model manager (future functionality)
            displayController.ClimateModelChanged += (s, e) => {
                ThermApp.ModelManager.UpdateDesiredClimate(e.Model);
            };

            // when the climate model changes, make sure to update the UX
            ThermApp.ModelManager.Subscribe(new FilterableChangeObserver<ClimateModelChangeResult, ClimateModel>(
                h => {
                    Console.WriteLine("UXController: Climate model changed, updating display.");
                    displayController.UpdateClimate(ThermApp.ModelManager.Climate);
                }
            ));

            Console.WriteLine("UXController Up");
        }

        protected void InitializePeripherals()
        {
            Console.WriteLine("UXController.InitializePeripherals");

            upButton = new PushButton(IOMap.Buttons.Device, IOMap.Buttons.UpPin);
            downButton = new PushButton(IOMap.Buttons.Device, IOMap.Buttons.DownPin);
            modeButton = new PushButton(IOMap.Buttons.Device, IOMap.Buttons.ModePin);

            upButton.Clicked += (s,e) => {
                // TODO: do some checks here:
                //if(this._climateModel.DesiredTemperature + 1 < someMax) {
                Console.WriteLine("Up button");
                ClimateModel newClimate = ClimateModel.From(ThermApp.ModelManager.Climate);
                newClimate.DesiredTemperature++;
                ThermApp.ModelManager.UpdateDesiredClimate(newClimate);
            };

            downButton.Clicked += (s,e) => {
                //if(this._climateModel.DesiredTemperature - 1 > someMin) {
                Console.WriteLine("Down button");
                ClimateModel newClimate = ClimateModel.From(ThermApp.ModelManager.Climate);
                newClimate.DesiredTemperature--;
                ThermApp.ModelManager.UpdateDesiredClimate(newClimate);
            };

            modeButton.Clicked += (s, e) => {
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