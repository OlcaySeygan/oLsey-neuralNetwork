using System;
using System.Collections.Generic;
using System.IO;

namespace oLseyLibrary.Model {
    public class Regression {
        public List<float[]> inputs = new List<float[]>();
        public List<float[]> outputs = new List<float[]>();

        public Regression() {

        }
        public Regression(string in_path, string out_path) {
            Add(in_path, out_path);
        }

        public void Add(float[] input, float[] output) {
            inputs.Add(input);
            outputs.Add(output);
        }
        public void Add(string in_path, string out_path) {
            string[] i_p = File.ReadAllLines(in_path);
            string[] o_p = File.ReadAllLines(out_path);
            if (i_p.Length != o_p.Length) return;

            int len = i_p.Length;
            for (int i = 0; i < len; i++) {
                string[] in_split = i_p[i].Split(' ');
                int in_len = in_split.Length;
                string[] out_split = o_p[i].Split(' ');
                int out_len = out_split.Length;
                float[] input = new float[in_len];
                float[] output = new float[out_len];

                for (int j = 0; j < in_len; j++) {
                    input[j] = float.Parse(in_split[j]);
                }
                for (int j = 0; j < out_len; j++) {
                    output[j] = float.Parse(out_split[j]);
                }

                inputs.Add(input);
                outputs.Add(output);
            }
        }
        public void Clear() {
            inputs.Clear();
            outputs.Clear();            
        }

        public int Length {
            get {
                return inputs.Count;
            }
        }

        public void Dispose() {
            GC.SuppressFinalize(this);
        }
    }
}
