using System;
using Meadow.Foundation.Sensors;
using Meadow.Peripherals.Sensors.Atmospheric;

namespace Therm
{
    /// <summary>
    /// The climate model is used throughout the entire app, so I wanted a central
    /// place in which to access it, and have all subscribers listen to changes
    /// to it in one place.
    /// </summary>
    public class ClimateModelManager : FilterableObservableBase<ClimateModelChangeResult, ClimateModel>
    {
        public event EventHandler<ClimateModelChangeResult> Updated = delegate { };

        public ClimateModel Climate {
            get { return _climate; }
            set {
                ClimateModelChangeResult result = new ClimateModelChangeResult(value, _climate);
                _climate = value;
                this.RaiseChangedAndNotify(result);
            }
        }
        protected ClimateModel _climate = new ClimateModel() {
            DesiredTemperature = 21,
            HvacOperatingMode = ClimateController.Mode.Off
        };

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static ClimateModelManager Instance { get => _instance.Value; }
        private static readonly Lazy<ClimateModelManager> _instance =
            new Lazy<ClimateModelManager>(
                () => new ClimateModelManager()
            );

        private ClimateModelManager()
        {
            Console.WriteLine("Instantiating ClimateModelManager");
        }

        /// <summary>
        /// Convenience method for updating the climate model.
        /// </summary>
        /// <param name="tempC"></param>
        public void UpdateAmbientTemp(float tempC)
        {
            Console.WriteLine("ClimateModelManager.UpdateAmbientTemp()");
            var updatedModel = ClimateModel.From(this._climate);
            updatedModel.CurrentConditions.Temperature = tempC;
            this.Climate = updatedModel;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="desiredClimate"></param>
        public void UpdateDesiredClimate(ClimateModel desiredClimate)
        {
            Console.WriteLine("ClimateModelManager.UpdateDesiredClimate()");
            // take the new HVAC settings from the passed in desired climate
            var updatedModel = ClimateModel.From(desiredClimate);
            // take the current conditions from what we already have
            updatedModel.CurrentConditions = AtmosphericConditions.From(this._climate.CurrentConditions);
            // update the climate, which will notify subs
            this.Climate = updatedModel;
        }

        protected void RaiseChangedAndNotify(ClimateModelChangeResult changeResult)
        {
            Updated?.Invoke(this, changeResult);
            base.NotifyObservers(changeResult);
        }
    }
}
