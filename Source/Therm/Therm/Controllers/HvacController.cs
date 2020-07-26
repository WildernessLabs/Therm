using System;
using System.Runtime.Remoting.Messaging;
using Meadow.Hardware;

namespace Therm
{
    /// <summary>
    /// Provides direct control of the heater, airconditioner, and fan
    /// components of the HVAC system.
    /// </summary>
    public class HvacController
    {
        public event EventHandler<HvacStatusResult> StatusChanged;

        protected IDigitalOutputPort heater;
        protected IDigitalOutputPort airConditioner;
        protected IDigitalOutputPort fan;

        public bool FanIsOn => fan.State;

        public bool AirConditionerIsOn => airConditioner.State;

        public bool HeaterIsOn => heater.State;

        public OperatingMode CurrentOperatingMode { get; protected set; } = OperatingMode.Off;

        public enum OperatingMode
        {
            Off,
            Heating,
            Cooling,
            FanOnly
        }

        public HvacController(
            IDigitalOutputPort heater,
            IDigitalOutputPort airConditioner,
            IDigitalOutputPort fan
            )
        {
            this.heater = heater;
            this.airConditioner = airConditioner;
            this.fan = fan;

            // start with everything off
            heater.State = false;
            airConditioner.State = false;
            fan.State = false;
        }

        public void TurnHeatOn()
        {
            if (AirConditionerIsOn) { airConditioner.State = false; }

            heater.State = true;
            fan.State = true;

            CurrentOperatingMode = OperatingMode.Heating;
            RaiseStatusEventAndNotify();
        }

        public void TurnHeatOff()
        {
            heater.State = false;
            CurrentOperatingMode = FanIsOn ? OperatingMode.FanOnly : OperatingMode.Off;
            RaiseStatusEventAndNotify();
        }

        public void TurnAirConditionerOn()
        {
            if (HeaterIsOn) { heater.State = false; }

            airConditioner.State = true;
            fan.State = true;

            CurrentOperatingMode = OperatingMode.Cooling;
            RaiseStatusEventAndNotify();
        }

        public void TurnAirConOff()
        {
            airConditioner.State = false;
            CurrentOperatingMode = FanIsOn ? OperatingMode.FanOnly : OperatingMode.Off;
            RaiseStatusEventAndNotify();
        }

        public void TurnFanOn()
        {
            fan.State = true;
            CurrentOperatingMode = OperatingMode.FanOnly;
            RaiseStatusEventAndNotify();
        }

        public void TurnFanOff()
        {
            // have to also turn off heating and cooling if no fan
            TurnAllOff();
        }

        public void TurnAllOff()
        {
            heater.State = false;
            airConditioner.State = false;
            fan.State = false;
            CurrentOperatingMode = OperatingMode.Off;
        }

        protected void RaiseStatusEventAndNotify()
        {
            StatusChanged?.Invoke(this, new HvacStatusResult(this.CurrentOperatingMode));
        }

        public class HvacStatusResult
        {
            OperatingMode NewMode { get; set; }

            public HvacStatusResult(OperatingMode newMode)
            {
                NewMode = newMode;
            }
        }
    }
}