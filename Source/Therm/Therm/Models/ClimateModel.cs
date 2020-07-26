using System;
using Meadow;
using Meadow.Peripherals.Sensors.Atmospheric;

namespace Therm
{
    public class ClimateModelChangeResult : IChangeResult<ClimateModel>
    {
        /// <summary>
        /// Current/new event value.
        /// </summary>
        public ClimateModel New { get; set; }
        /// <summary>
        /// Previous value.
        /// </summary>
        public ClimateModel Old { get; set; }

        public ClimateModelChangeResult(ClimateModel newValue, ClimateModel oldValue)
        {
            this.New = newValue;
            this.Old = oldValue;
        }
    }

    public class ClimateModel
    {
        public float DesiredTemperature { get; set; }

        public ClimateController.Mode HvacOperatingMode {
            get; set;
        } = ClimateController.Mode.Off;

        public AtmosphericConditions CurrentConditions {
            get; set;
        } = new AtmosphericConditions();

        public ClimateModel()
        {
        }

        public ClimateModel(
            float desiredTemperature,
            ClimateController.Mode hvacOperatingMode,
            AtmosphericConditions currentConditions
            )
        {
            this.DesiredTemperature = desiredTemperature;
            this.HvacOperatingMode = hvacOperatingMode;
            this.CurrentConditions = currentConditions;
        }

        public static ClimateModel From(ClimateModel model)
        {
            return new ClimateModel(
                model.DesiredTemperature,
                model.HvacOperatingMode,
                AtmosphericConditions.From(model.CurrentConditions)
                );
        }

    }

    public class ClimateModelResult : EventArgs
    {
        public ClimateModel Model { get; set; }

        public ClimateModelResult(ClimateModel model)
        {
            this.Model = model;
        }
    }
}
