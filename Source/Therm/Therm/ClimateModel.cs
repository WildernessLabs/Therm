using System;
using Meadow.Peripherals.Sensors.Atmospheric;

namespace Therm
{
    public class ClimateModel
    {
        public float DesiredTemperature { get; set; }

        public ClimateController.Mode HvacOperatingMode { get; set; }

        public AtmosphericConditions CurrentConditions { get; set; }

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
