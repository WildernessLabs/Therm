using System;
using Meadow.Hardware;

namespace Therm
{
    /// <summary>
    /// Provides direct control of the heater, airconditioner, and fan
    /// components of the HVAC system.
    /// </summary>
    public class HvacController
    {
        public event EventHandler<HvacStatusResult> StatusChanged = delegate { };

        protected IDigitalOutputPort _heater;
        protected IDigitalOutputPort _airCon;
        protected IDigitalOutputPort _fan;

        public bool FanIsOn {
            get { return _fan.State; }
        }

        public bool AirConIsOn {
            get { return _airCon.State; }
        }

        public bool HeaterIsOn {
            get { return _heater.State; }
        }

        public OperatingMode CurrentOperatingMode {
            get; protected set;
        } = OperatingMode.Off;


        public HvacController(
            IDigitalOutputPort heater,
            IDigitalOutputPort airCon,
            IDigitalOutputPort fan
            )
        {
            this._heater = heater;
            this._airCon = airCon;
            this._fan = fan;

            // start with everything off
            this._heater.State = false;
            this._airCon.State = false;
            this._fan.State = false;
        }

        public void TurnHeatOn()
        {
            if (AirConIsOn) { _airCon.State = false; }

            _heater.State = true;
            _fan.State = true;

            CurrentOperatingMode = OperatingMode.Heating;
            this.RaiseStatusEventAndNotify();
        }

        public void TurnHeatOff()
        {
            _heater.State = false;
            CurrentOperatingMode = FanIsOn ? OperatingMode.FanOnly : OperatingMode.Off;
            this.RaiseStatusEventAndNotify();
        }

        public void TurnAirConOn()
        {
            if (HeaterIsOn) { _heater.State = false; }

            _airCon.State = true;
            _fan.State = true;

            CurrentOperatingMode = OperatingMode.Cooling;
            this.RaiseStatusEventAndNotify();
        }

        public void TurnAirConOff()
        {
            _airCon.State = false;
            CurrentOperatingMode = FanIsOn ? OperatingMode.FanOnly : OperatingMode.Off;
            this.RaiseStatusEventAndNotify();
        }

        public void TurnFanOn()
        {
            _fan.State = true;
            CurrentOperatingMode = OperatingMode.FanOnly;
            this.RaiseStatusEventAndNotify();
        }

        public void TurnFanOff()
        {
            // have to also turn off heating and cooling if no fan
            this.TurnAllOff();
        }

        public void TurnAllOff()
        {
            _heater.State = false;
            _airCon.State = false;
            _fan.State = false;
            CurrentOperatingMode = OperatingMode.Off;
        }

        public enum OperatingMode
        {
            Off,
            Heating,
            Cooling,
            FanOnly
        }

        protected void RaiseStatusEventAndNotify()
        {
            StatusChanged?.Invoke(this, new HvacStatusResult(this.CurrentOperatingMode));
        }

        public class HvacStatusResult
        {
            HvacController.OperatingMode NewMode { get; set; }

            public HvacStatusResult(OperatingMode newMode)
            {
                NewMode = newMode;
            }
        }
    }
}
