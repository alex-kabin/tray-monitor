using System;

namespace Sensor.HandsFreeBattery
{
    [Flags]
    public enum AppleAccessoryFeatures
    {
        NONE = 0,
        
        // The accessory supports battery reporting (reserved only for battery operated accessories).
        BATTERY_REPORTING = 1 << 1,
        
        // The accessory is docked or powered (reserved only for battery operated accessories).
        DOCKED_OR_POWERED = 1 << 2,
        
        // The accessory supports Siri status reporting.
        SIRI_STATUS_REPORTING = 1 << 3,
        
        // The accessory supports noise reduction (NR) status reporting.
        NR_STATUS_REPORTING = 1 << 4
    }
}