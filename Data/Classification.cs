using System.Collections.Generic;

namespace oLseyLibrary.Model {
    public class Classification {
        public List<float[]> inputs = new List<float[]>();
        public List<string> tags = new List<string>();

        public int Length {
            get {
                return inputs.Count;
            }
        }

        public void Add(float[] input, string tag) {
            inputs.Add(input);
            tags.Add(tag);
        }
        public void Clear() {
            inputs.Clear();
            tags.Clear();
        }
    }
}
