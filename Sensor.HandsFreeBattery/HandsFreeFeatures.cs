using System;

namespace Sensor.HandsFreeBattery
{
    [Flags]
    public enum HandsFreeFeatures
    {
        NONE = 0,
        EC_NR_FUNCTION = 1 << 0,
        THREE_WAY_CALLING = 1 << 1,
        CLI_PRESENTATION__CAPABILITY = 1 << 2,
        VOICE_RECOGNITION_ACTIVATION = 1 << 3,
        REMOTE_VOLUME_CONTROL = 1 << 4,
        ENHANCED_CALL_STATUS = 1 << 5,
        ENHANCED_CALL_CONTROL  = 1 << 6,
        CODEC_NEGOTIATION  = 1 << 7,
        HF_INDICATORS  = 1 << 8,
        ESCO_SETTINGS  = 1 << 9,
        RESERVED1  = 1 << 10,
        RESERVED2  = 1 << 11,
    }
}