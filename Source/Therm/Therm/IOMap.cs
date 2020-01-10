﻿using System;
using Meadow.Hardware;

namespace Therm
{
    public static class IOMap
    {
        // Display
        public static (IIODevice IODevice,
                        IPin ClockPin,
                        IPin MosiPin,
                        IPin DCPin,
                        IPin ResetPin) Display = (
            ThermApp.Device,
            ThermApp.Device.Pins.SCK,
            ThermApp.Device.Pins.MISO,
            ThermApp.Device.Pins.D01,
            ThermApp.Device.Pins.D00);

        // temp sensor
        public static (IIODevice Device, IPin Pin) AnalogTempSensor = (
            ThermApp.Device, ThermApp.Device.Pins.A00);

        // HVAC Control
        public static (IIODevice Device,
                        IPin HeaterPin,
                        IPin AirConPin,
                        IPin FanPin) HVac = (
            ThermApp.Device,
            ThermApp.Device.Pins.D10,
            ThermApp.Device.Pins.D11,
            ThermApp.Device.Pins.D12);

        // Buttons
        public static (IIODevice Device,
                        IPin UpPin,
                        IPin DownPin,
                        IPin ModePin) Buttons = (
            ThermApp.Device,
            ThermApp.Device.Pins.D03,
            ThermApp.Device.Pins.D04,
            ThermApp.Device.Pins.D05);

        static IOMap()
        {
        }
    }
}