using System;
using Meadow;
using Meadow.Foundation.Sensors.Temperature;
using Meadow.Peripherals.Sensors.Atmospheric;

namespace Therm
{
    public class AmbientConditionsListener
    {
        AnalogTemperature temperatureSensor;
        public static ClimateModelManager ModelManager { get => ClimateModelManager.Instance; }

        public AmbientConditionsListener()
        {
            Initialize();

            // take an intial reading
            var temp = temperatureSensor.Read().Result.Temperature;
            ModelManager.UpdateAmbientTemp(temp ?? 0);

            Console.WriteLine("Starting up the temp sensor.");
            temperatureSensor.StartUpdating(standbyDuration: 1000);

            Console.WriteLine("Temp sensor spinning");
        }

        protected void Initialize()
        {
            temperatureSensor = new AnalogTemperature(
            IODeviceMap.AnalogTempSensor.Device,
            IODeviceMap.AnalogTempSensor.Pin,
            AnalogTemperature.KnownSensorType.TMP35);

            // subscribe to 1/4º C changes in temp
            temperatureSensor.Subscribe(new FilterableChangeObserver<AtmosphericConditionChangeResult, AtmosphericConditions>(
                h => {
                    // probably update screen or something
                    Console.WriteLine($"Update from temp sensor: {h.New.Temperature}ºC");
                    ModelManager.UpdateAmbientTemp(h.New.Temperature.Value);
                },
                e => {
                    return Math.Abs(e.Delta.Temperature.Value) > 0.25f;
                }));
        }

    }
}