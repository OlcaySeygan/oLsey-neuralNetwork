using System.Collections.Generic;

using oLseyLibrary.Enums;

namespace oLseyLibrary.Model.Components {
    public class Layer {
        protected List<Perceptron> perceptrons = new List<Perceptron>();
        protected Perceptron bias = null;

        #region Properties
        protected PerceptronType.Type type = PerceptronType.Type.hidden;

        public float[] input {
            get {
                int length = Length;
                float[] inputs = new float[length];

                for (int i = 0; i < length; i++) {
                    inputs[i] = perceptrons[i].state;
                }

                return inputs;
            }
            set {
                int length = Length;

                for (int i = 0; i < length; i++) {
                    perceptrons[i].state = value[i];
                }
            }
        }
        public float[] output {
            get {
                int length = Length;
                float[] outputs = new float[length];

                for (int i = 0; i < length; i++) {
                    outputs[i] = perceptrons[i].activation();
                }

                return outputs;
            }
        }
        public float[] desired {
            set {
                if (!type.Equals(PerceptronType.Type.output))
                    return;

                int length = Length;

                for (int i = 0; i < length; i++) {
                    perceptrons[i].desired = value[i];
                }
            }
        }
        public float[] error {
            get {
                int length = Length;
                float[] errors = new float[length];

                for (int i = 0; i < length; i++) {
                    errors[i] = perceptrons[i].error;
                }

                return errors;
            }
        }
        #endregion

        public Layer(int len, bool bias_enable, ActivationType.Type activation_type, PerceptronType.Type perceptron_type) {
            type = perceptron_type;
            for (int i = 0; i < len; i++) {
                perceptrons.Add(new Perceptron() {
                    activation_type = activation_type,
                    type = type,
                    tag = "untitled",
                    current_layer = this
                });
            }
            if (bias_enable) {
                bias = new Perceptron() {
                    type = PerceptronType.Type.bias,
                    tag = "bias",
                    state = 1f,
                    current_layer = this
                };
            }
        }

        public void FeedForward() {
            int length = Length;
            for (int i = 0; i < length; i++) {
                perceptrons[i].FeedForward();
            }
        }
        public void BlackError() {
            int length = Length;
            for (int i = 0; i < length; i++) {
                perceptrons[i].BlackError();
            }
            if (!IsBiasNull())
                bias.BlackError();
        }
        public void BackPropagation(float lr, float mr) {
            int length = Length;
            for (int i = 0; i < length; i++) {
                perceptrons[i].BackPropagation(lr, mr);
            }
            if (!IsBiasNull())
                bias.BackPropagation(lr, mr);
        }

        public Perceptron[] GetPerceptrons() {
            return perceptrons.ToArray();
        }
        public Perceptron GetBias() {
            return bias;
        }

        public bool IsBiasNull() {
            return bias == null;
        }

        public static void Connect(Layer main, Layer other) {
            if (!main.IsBiasNull()) {
                Perceptron.Connect(main.bias, other.perceptrons.ToArray());
            }
            Perceptron.Connect(main.perceptrons.ToArray(), other.perceptrons.ToArray());
        }
        public static void Connect(Layer main, Layer other, Synapse[,] synapses) {
            if (!main.IsBiasNull()) {
                Perceptron.Connect(main.bias, other.perceptrons.ToArray());
            }
            Perceptron.Connect(main.perceptrons.ToArray(), other.perceptrons.ToArray());
        }

        public int Length {
            get {
                return perceptrons.Count;
            }
        }
        public Perceptron this[int index] {
            get { return perceptrons[index]; }
        }
    }
}
