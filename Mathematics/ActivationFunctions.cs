using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace oLseyLibrary.Mathematics {
    public class ActivationFunctions {
        public static float Identity(float x, bool derivate = false) {
            return derivate ? 1f : x;
        }
        public static float Sigmoid(float x, bool derivate = false) {
            var fx = 1f / (1f + (float)Math.Exp(-x));
            if (!derivate)
                return fx;
            return fx * (1f - fx);
        }
        public static float TanH(float x, bool derivate = false) {
            if (derivate)
                return 1f - (float)Math.Pow(Math.Tanh(x), 2);
            return (float)Math.Tanh(x);
        }
        public static float RelU(float x, bool derivate = false) {
            if (derivate)
                return x > 0f ? 1f : 0f;
            return x > 0f ? x : 0f;
        }
        public static float LeakyReLU(float x, bool derivate = false) {
            var alpha = 0.01f;
            if (derivate)
                return x > 0f ? 1f : alpha;
            return x > 0f ? x : x * alpha;
        }
    }
}
