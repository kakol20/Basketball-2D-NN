namespace Own
{
    public static class Math
    {
        private static float Constrain(float val, float low, float high)
        {
            return UnityEngine.Mathf.Clamp(val, low, high);
            //return UnityEngine.Mathf.Max(UnityEngine.Mathf.Min(val, high), low);
        }

        public static float Map(float val, float minA, float maxA, float minB, float maxB, bool withinBounds = false)
        {
            float newVal = (val - minA) / (maxA - minA) * (maxB - minB) + minB;

            if (!withinBounds) return newVal;

            if (minB < maxB)
            {
                return Constrain(newVal, minB, maxB);
            }
            else
            {
                return Constrain(newVal, maxB, minB);
            }
        }
    }

    public static class Random
    {
        /// <summary>
        /// Set Random Seed
        /// </summary>
        public static void Init() => UnityEngine.Random.InitState(System.DateTime.Now.Millisecond);
        public static void Init(int seed) => UnityEngine.Random.InitState(seed);

        /// <summary>
        /// Randomisation between min and max
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float Range(float min = 0, float max = 1)
        {
            float range = max - min;
            float value = UnityEngine.Random.value;

            return (range * value) + min;
        }
    }
}