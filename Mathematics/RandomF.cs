using System;

namespace oLseyLibrary.Mathematics {
    public class RandomF {
        private static readonly System.Random random = new System.Random();
        public static float NextFloat() {
            return (float)(random.NextDouble() * (1f - 0f) + 0f);
        }
        public static float NextFloat(float max) {
            return (float)(random.NextDouble() * (max - 0f) + 0f);
        }
        public static float NextFloat(float min, float max) {
            return (float)(random.NextDouble() * (max - min) + min);
        }

        public static int Next() {
            return random.Next(2);
        }
        public static int Next(int max) {
            return random.Next(0, max);
        }
        public static int Next(int min, int max) {
            return random.Next(min, max);
        }

        public static float NextGaussian(float mu = 0, float sigma = 1) {
            if (sigma <= 0)
                throw new ArgumentOutOfRangeException("sigma", "Must be greater than zero.");
            
            float v1, v2, rSquared;
            do {
                v1 = 2 * NextFloat() - 1;
                v2 = 2 * NextFloat() - 1;
                rSquared = v1 * v1 + v2 * v2;
            } while (rSquared >= 1 || rSquared == 0);
            
            var polar = (float)Math.Sqrt(-2 * Math.Log(rSquared) / rSquared);
            
            return v1 * polar * sigma + mu;
        }
    }
}
