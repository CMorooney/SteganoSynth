namespace ImageMusic
{
    public static class PrimitiveExtensions
    {
        /// <summary>
        /// Converts a float value known to be in one range to another range
        /// </summary>
        /// <returns>The float adjusted for the desired range</returns>
        /// <param name="currentMin">The minimum value of the known range</param>
        /// <param name="currentMax">The maximum value of the known range</param>
        /// <param name="desiredMin">The minimum value of the desired range</param>
        /// <param name="desiredMax">The maximum value of the desired range</param>
        public static float ConvertToRange(this float @value,
                                           float currentMin, float currentMax,
                                           float desiredMin, float desiredMax)
        {
            var currentRange = currentMax - currentMin;
            var desiredRange = desiredMax - desiredMin;
            return (((@value - currentMin) * desiredRange) / currentRange) + desiredMin;
        }
    }
}