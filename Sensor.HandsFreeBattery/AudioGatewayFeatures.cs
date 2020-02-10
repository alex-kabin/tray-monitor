using System;

namespace Sensor.HandsFreeBattery
{
    [Flags]
    public enum AudioGatewayFeatures
    {
        NONE = 0,
        THREE_WAY_CALLING = 1 << 0,
        EC_NR_FUNCTION = 1 << 1,
        VOICE_RECOGNITION_FUNCTION = 1 << 2,
        IN_BAND_RING_TONE_CAPABILITY = 1 << 3,
        ATTACH_A_NUMBER_TO_A_VOICE_TAG = 1 << 4,
        ABILITY_TO_REJECT_A_CALL = 1 << 5,
        ENHANCED_CALL_STATUS = 1 << 6,
        ENHANCED_CALL_CONTROL = 1 << 7,
        EXTENDED_ERROR_RESULT_CODES = 1 << 8,
        CODEC_NEGOTIATION = 1 << 9,
        HF_INDICATORS = 1 << 10,
        ESCO_SETTINGS  = 1 << 11,
    }
}