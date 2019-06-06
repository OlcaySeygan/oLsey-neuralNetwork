using System.Collections.Generic;

namespace oLseyLibrary.Model {
    public class Test {
        public List<float[]> inputs = new List<float[]>();
        public List<float[]> outputs = new List<float[]>();

        public void Add(float[] input, float[] output) {
            inputs.Add(input);
            outputs.Add(output);
        }
    }
}
