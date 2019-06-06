using System;

namespace oLseyLibrary.Mathematics {
    public class GaussianRandom {
        private bool hasDeviate;
        private float storedDeviate;
        
        public float NextGaussian(float mu = 0, float sigma = 1) {
            if (sigma <= 0)
                throw new ArgumentOutOfRangeException("sigma", "Must be greater than zero.");

            if (hasDeviate) {
                hasDeviate = false;
                return storedDeviate * sigma + mu;
            }

            float v1, v2, rSquared;
            do {
                v1 = 2 * RandomF.NextFloat() - 1;
                v2 = 2 * RandomF.NextFloat() - 1;
                rSquared = v1 * v1 + v2 * v2;
            } while (rSquared >= 1 || rSquared == 0);

            
            var polar = (float)Math.Sqrt(-2 * Math.Log(rSquared) / rSquared);
            
            storedDeviate = v2 * polar;
            hasDeviate = true;
            
            return v1 * polar * sigma + mu;
        }
    }
}
