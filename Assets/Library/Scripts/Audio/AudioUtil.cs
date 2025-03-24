using UnityEngine;

namespace Library.Scripts.Audio
{
    public static class AudioUtil {
        /// <summary>
        /// Usage:
        /// float value = 1;
        /// float db = value.LinearToDecibel();
        /// </summary>
        /// <param name="linear">Value from 0 -> 1</param>
        /// <returns></returns>
        public static float LinearToDecibel(this float linear) => linear != 0 ? 20.0f * Mathf.Log10(linear) : -144.0f;
        
        /// <summary>
        /// Usage:
        /// float value = -80;
        /// float volume = value.DecibelToLinear();
        /// </summary>
        /// <param name="dB">Value from -80db -> max db value</param>
        /// <returns></returns>
        public static float DecibelToLinear(this float dB) => Mathf.Pow(10.0f, dB / 20.0f);
    }
}