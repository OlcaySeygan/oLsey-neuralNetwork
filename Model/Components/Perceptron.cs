using System;
using System.Collections.Generic;

using oLseyLibrary.Enums;
using oLseyLibrary.Mathematics;

namespace oLseyLibrary.Model.Components {
    public class Perceptron : IComparable<Perceptron> {
        private List<Perceptron> perceptrons_to = new List<Perceptron>();
        private List<Synapse> synapses_to = new List<Synapse>();
        private List<Perceptron> perceptrons_from = new List<Perceptron>();
        private List<Synapse> synapses_from = new List<Synapse>();
        private List<int> index_in_other_perceptrons = new List<int>();

        public Layer current_layer = null;
        
        public PerceptronType.Type type = PerceptronType.Type.hidden;
        public ActivationType.Type activation_type = ActivationType.Type.sigmoid;
        public float state = 0f;
        public float desired = 0f;
        public float error = 0f;
        public string tag = "untitled";
        
        public float activation(bool derivate = false) {
            if (type == PerceptronType.Type.input || type == PerceptronType.Type.bias)
                return state;

            if(activation_type == ActivationType.Type.sigmoid)
                return ActivationFunctions.Sigmoid(state, derivate);
            else if (activation_type == ActivationType.Type.relu)
                return ActivationFunctions.RelU(state, derivate);
            else if(activation_type == ActivationType.Type.tanh)
                return ActivationFunctions.TanH(state, derivate);
            else if (activation_type == ActivationType.Type.identity)
                return ActivationFunctions.Identity(state, derivate);
            else if (activation_type == ActivationType.Type.lrelu)
                return ActivationFunctions.LeakyReLU(state, derivate);
            else
                return ActivationFunctions.Sigmoid(state, derivate);
            
        }

        public void FeedForward() {
            if(type == PerceptronType.Type.hidden || type == PerceptronType.Type.output) {
                state = 0f;
                int len = perceptrons_from.Count;
                for (int i = 0; i < len; i++) {
                    Perceptron perc = perceptrons_from[index_in_other_perceptrons[i]];
                    Synapse syn = synapses_from[index_in_other_perceptrons[i]];
                    state += perc.activation() * syn.x;
                }
            }
        }
        public void BlackError() {
            var x = activation();
            var derivate = activation(true);

            if(type == PerceptronType.Type.output) {
                error = (desired - x) * derivate;
            } else {
                int len = perceptrons_to.Count;

                float er = 0f;
                for (int i = 0; i < len; i++) {
                    Perceptron perc = perceptrons_to[i];
                    Synapse syn = synapses_to[i];
                    er += perc.error * syn.x;
                }

                error = derivate * er;
            }
        }
        public void BackPropagation(float lr, float mr) {
            int len = perceptrons_to.Count;
            
            for (int i = 0; i < len; i++) {
                Perceptron perc = perceptrons_to[i];
                Synapse syn = synapses_to[i];
                syn.error = (lr * activation() * perc.error) + (mr * syn.error);
                syn.x += syn.error;
            }
        }

        #region Static
        public static void Connect(Perceptron main, Perceptron perceptron) {
            Connect(main, perceptron, new Synapse());
        }
        public static void Connect(Perceptron main, Perceptron[] perceptrons) {
            for (int i = 0; i < perceptrons.Length; i++) {
                var perceptron = perceptrons[i];
                Connect(main, perceptron);
            }
        }
        public static void Connect(Perceptron[] mains, Perceptron perceptron) {
            for (int i = 0; i < mains.Length; i++) {
                var main = mains[i];
                Connect(main, perceptron);
            }
        }
        public static void Connect(Perceptron[] mains, Perceptron[] perceptrons) {
            for (int i = 0; i < mains.Length; i++) {
                var main = mains[i];
                for (int j = 0; j < perceptrons.Length; j++) {
                    var perceptron = perceptrons[j];
                    Connect(main, perceptron);
                }
            }
        }
        public static void Connect(Perceptron main, Perceptron perceptron, Synapse synapse) {
            main.perceptrons_to.Add(perceptron);
            main.synapses_to.Add(synapse);

            perceptron.perceptrons_from.Add(main);
            perceptron.synapses_from.Add(synapse);

            perceptron.index_in_other_perceptrons.Add(perceptron.perceptrons_from.Count - 1);
        }
        public static void Connect(Perceptron main, Perceptron[] perceptrons, Synapse[] synapses) {
            for (int i = 0; i < perceptrons.Length; i++) {
                var perceptron = perceptrons[i];
                var synapse = synapses[i];
                Connect(main, perceptron, synapse);
            }
        }
        public static void Connect(Perceptron[] mains, Perceptron perceptron, Synapse[] synapses) {
            for (int i = 0; i < mains.Length; i++) {
                var main = mains[i];
                var synapse = synapses[i];
                Connect(main, perceptron, synapse);
            }
        }
        public static void Connect(Perceptron[] mains, Perceptron[] perceptrons, Synapse[,] synapses) {
            for (int i = 0; i < mains.Length; i++) {
                var main = mains[i];
                for (int j = 0; j < perceptrons.Length; j++) {
                    var perceptron = perceptrons[j];
                    var synapse = synapses[i, j];
                    Connect(main, perceptron, synapse);
                }
            }
        }
        #endregion
        
        public int CompareTo(Perceptron other) {
            if (other.Equals(null)) return 1;
            if (activation() > other.activation()) return -1;
            else if (activation() < other.activation()) return 1;
            else return 0;
        }

        public int Length {
            get {
                return synapses_to.Count;
            }
        }
        public Synapse this[int index] {
            get { return synapses_to[index]; }
        }
    }
}
