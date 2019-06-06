using System;
using System.Collections.Generic;
using System.IO;

using oLseyLibrary.Enums;
using oLseyLibrary.Mathematics;
using oLseyLibrary.Model;
using oLseyLibrary.Model.Components;

namespace oLseyLibrary {
    public class NeuralNetwork : IComparable<NeuralNetwork> {
        private Layer[] layers;
        private Layer input_layer = null;
        private Layer[] hidden_layers = null;
        private Layer output_layer = null;
        
        public float learning_rate;
        public float momentum_rate;
        public float error_limit;
        public bool ignore_error;
        public bool infinite_loop;
        public int max_circle;
        public int circle;
        public float score;
        public float fitness;
        public int id;
        
        private void SetInput(params float[] values) {
            for (int i = 0; i < values.Length; i++) {
                input_layer[i].state = values[i];
            }
        }
        private float GetError() {
            float error = 0f;
            foreach (var layer in layers) {
                foreach (var perc in layer.GetPerceptrons()) {
                    error += Math.Abs(perc.error);
                }
            }
            return error;
        }
        public float total_error = float.PositiveInfinity;

        public string[] GetTag() {
            string[] tags = new string[output_layer.Length];
            for (int i = 0; i < tags.Length; i++) {
                tags[i] = output_layer[i].tag;
            }
            return tags;
        }
        public void SetTag(params string[] tag) {
            int len = output_layer.Length;
            for (int i = 0; i < len; i++) {
                output_layer[i].tag = tag[i];
            }
        }

        public NeuralNetwork(params int[] units) {
            learning_rate = 0.03f;
            momentum_rate = 0.3f;
            error_limit = 0.005f;
            ignore_error = false;
            infinite_loop = false;
            max_circle = int.MaxValue;
            circle = 0;
            score = 0f;
            fitness = 0f;
            id = 0;

            layers = new Layer[units.Length];

            for (int i = 0; i < layers.Length; i++) {
                PerceptronType.Type type = PerceptronType.Type.hidden;
                if (i == 0) type = PerceptronType.Type.input;
                else if (i == layers.Length - 1) type = PerceptronType.Type.output;
                layers[i] = new Layer(units[i], (i != 0), ActivationType.Type.sigmoid, type);
            }

            input_layer = this.layers[0];
            hidden_layers = new Layer[layers.Length - 2];
            for (int i = 0; i < hidden_layers.Length; i++) {
                hidden_layers[i] = this.layers[i + 1];
            }
            output_layer = this.layers[this.layers.Length - 1];

            for (int i = 0; i < layers.Length - 1; i++) {
                Layer.Connect(this.layers[i], this.layers[i + 1]);
            }
        }
        public NeuralNetwork(NeuralNetwork other) {
            learning_rate = other.learning_rate;
            momentum_rate = other.momentum_rate;
            error_limit = other.error_limit;
            ignore_error = other.ignore_error;
            infinite_loop = other.infinite_loop;
            max_circle = other.max_circle;
            circle = other.circle;
            score = other.score;
            fitness = other.fitness;
            id = other.id;

            layers = new Layer[other.layers.Length];
            for (int i = 0; i < other.layers.Length; i++) {
                layers[i] = null;
            }
            input_layer = layers[0];
            output_layer = layers[layers.Length - 1];

            string[] tags = new string[this.output_layer.Length];
            for (int i = 0; i < this.output_layer.Length; i++) {
                tags[i] = other.output_layer[i].tag;
            }
            SetTag(tags);

            for (int i = 0; i < other.layers.Length - 1; i++) {
                Layer.Connect(layers[i], layers[i + 1]);
            }

            for (int i = 0; i < layers.Length; i++) {
                for (int j = 0; j < layers[i].Length; j++) {
                    for (int k = 0; k < layers[i][j].Length; k++) {
                        layers[i][j][k].x = other.layers[i][j][k].x;
                    }
                }
                if (!layers[i].IsBiasNull())
                    for (int k = 0; k < layers[i].GetBias().Length; k++) {
                        layers[i].GetBias()[k].x = other.layers[i].GetBias()[k].x;
                    }
            }
        }

        private void FeedForward() {
            for (int i = 0; i < layers.Length; i++) {
                layers[i].FeedForward();
            }
        }
        private void BlackError() {
            for (int i = layers.Length - 1; i >= 0; i--) {
                layers[i].BlackError();
            }
        }
        private void BackPropagation() {
            for (int i = layers.Length - 1; i >= 0; i--) {
                layers[i].BackPropagation(learning_rate, momentum_rate);
            }
        }

        public Result[] Predict(params float[] inputs) {
            SetInput(inputs);
            FeedForward();
            var results = new Result[output_layer.Length];
            var tags = GetTag();
            for (int i = 0; i < results.Length; i++) {
                results[i] = new Result(output_layer[i].activation(), tags[i]);
            }
            return results;
        }
        public List<Result[]> Predict_TrainingData(Regression training_data) {
            var results_list = new List<Result[]>();
            for (int i = 0; i < training_data.Length; i++) {
                SetInput(training_data.inputs[i]);
                FeedForward();
                var results = new Result[output_layer.Length];
                var tags = GetTag();
                for (int j = 0; j < results.Length; j++) {
                    results[j] = new Result(output_layer[j].activation(), tags[j]);
                }
                results_list.Add(results);
            }
            return results_list;
        }
        public Result[] Classifier(float[] inputs) {
            SetInput(inputs);
            FeedForward();

            List<Perceptron> list = new List<Perceptron>();
            list.AddRange(output_layer.GetPerceptrons());
            list.Sort();

            int len = list.Count;
            Result[] guesses = new Result[len];
            for (int i = 0; i < len; i++) {
                guesses[i] = new Result(list[i].activation(), list[i].tag);
            }
            return guesses;
        }
        public Result[] SoftMax(Result[] guesses) {
            float sum = 0f;
            for (int i = 0; i < guesses.Length; i++) {
                sum += guesses[i].value;
            }
            for (int i = 0; i < guesses.Length; i++) {
                guesses[i].value = guesses[i].value / sum;
            }
            return guesses;
        }

        #region Training
        private void Training(float[] inputs, float[] outputs) {
            input_layer.input = inputs;
            output_layer.desired = outputs;

            FeedForward();
            BlackError();
            BackPropagation();
        }
        public void Training(Regression training_data, int batch_size = 128) {
            if (batch_size <= 0) return;
            if (batch_size > training_data.Length) batch_size = training_data.Length;

            circle = 0;
            do {
                total_error = 0f;
                
                var batch_counter = 0;
                var batch_limit = batch_size;
                while (batch_counter < training_data.Length) {
                    var batch_regression = new Regression();
                    for (var i = batch_counter; i < batch_limit && i < training_data.Length; i++) {
                        batch_regression.Add(training_data.inputs[i], training_data.outputs[i]);
                    }

                    var len = batch_regression.Length;
                    for (var i = 0; i < len; i++) {
                        Training(batch_regression.inputs[i], batch_regression.outputs[i]);
                        total_error += GetError();
                    }
                    batch_counter += batch_size;
                    batch_limit += batch_size;
                    batch_regression.Dispose();
                }

                if (!infinite_loop)
                    if (circle >= max_circle)
                        break;
                circle++;
            } while (!ignore_error && (total_error >= error_limit));
        }
        public bool Training_Loop(Regression training_data, int step_size = 1, int batch_size = 128) {
            if (step_size <= 0 || batch_size <= 0) return false;

            if (batch_size > training_data.Length) batch_size = training_data.Length;

            for (int s = 0; s < step_size; s++) {

                if (!infinite_loop)
                    if (circle >= max_circle)
                        return false;

                if (!ignore_error)
                    if (total_error <= error_limit)
                        return false;

                total_error = 0f;

                var batch_counter = 0;
                var batch_limit = batch_size;
                while (batch_counter < training_data.Length) {                    
                    var batch_regression = new Regression();
                    for (var i = batch_counter; i < batch_limit && i < training_data.Length; i++) {
                        batch_regression.Add(training_data.inputs[i], training_data.outputs[i]);
                    }
                    
                    var len = batch_regression.Length;
                    for (var i = 0; i < len; i++) {
                        Training(batch_regression.inputs[i], batch_regression.outputs[i]);
                        total_error += GetError();
                    }
                    batch_counter += batch_size;
                    batch_limit += batch_size;
                }
                
                circle++;
            }
            return true;
        }
        public void Training(Classification trainingData) {
            Regression regressionTrainingData = new Regression();
            for (int i = 0; i < trainingData.Length; i++) {
                float[] outputs = new float[output_layer.Length];
                for (int j = 0; j < outputs.Length; j++) {
                    if (trainingData.tags[i].Equals(output_layer[j].tag))
                        outputs[j] = 1f;
                    else outputs[j] = 0f;
                }
                regressionTrainingData.Add(trainingData.inputs[i], outputs);
            }
            Training(regressionTrainingData);
        }
        #endregion

        #region SaveLoad
        public void SaveToFile(string path) {
            if (File.Exists(path))
                File.Delete(path);
            File.WriteAllLines(path, SaveLines());
        }
        public string[] SaveLines() {
            List<string> lines = new List<string>();
            int len = layers.Length;

            string line_learning_rate = "learning_rate=" + learning_rate;
            lines.Add(line_learning_rate);

            string line_momentum_rate = "momentum_rate=" + momentum_rate;
            lines.Add(line_momentum_rate);

            string line_error_limit = "error_limit=" + error_limit;
            lines.Add(line_error_limit);

            string line_ignore_error = "ignore_error=" + ignore_error;
            lines.Add(line_ignore_error);

            string line_infinite_loop = "infinite_loop=" + infinite_loop;
            lines.Add(line_infinite_loop);

            string line_max_circle = "max_circle=" + max_circle;
            lines.Add(line_max_circle);

            string line_circle = "circle=" + circle;
            lines.Add(line_circle);

            string line_score = "score=" + score;
            lines.Add(line_score);

            string line_fitness = "fitness=" + fitness;
            lines.Add(line_fitness);

            string line_input = "input_layer=" + input_layer.Length;
            lines.Add(line_input);
            string line_hiddens = "hidden_layers=";
            for (int i = 0; i < hidden_layers.Length; i++) {
                if (i != 0) line_hiddens += " ";
                line_hiddens += hidden_layers[i].Length;
            }
            lines.Add(line_hiddens);
            string line_output = "output_layer=" + output_layer.Length;
            lines.Add(line_output);


            string[] lines_synapses = new string[len - 1];
            for (int i = 0; i < lines_synapses.Length; i++) {
                len = this.layers[i].Length;
                int next_len = this.layers[i + 1].Length;

                lines_synapses[i] = "synapses_" + i.ToString() + ":" + len + "x" + next_len + "=[";
                for (int x = 0; x < len; x++) {
                    for (int y = 0; y < next_len; y++) {
                        if (!(x == 0 && y == 0))
                            lines_synapses[i] += " ";
                        lines_synapses[i] += x + "," + y + ":" + layers[i][x][y].x.ToString();
                    }
                }
                lines_synapses[i] += "]";
                lines.Add(lines_synapses[i]);
            }

            string[] lines_biases = new string[lines_synapses.Length];
            for (int i = 0; i < lines_biases.Length; i++) {
                len = this.layers[i + 1].Length;
                lines_biases[i] = "biases_" + i.ToString() + ":" + len + "=[";
                for (int j = 0; j < layers[i].GetBias().Length; j++) {
                    if (!(j == 0))
                        lines_biases[i] += " ";
                    lines_biases[i] += layers[i].GetBias()[j].x.ToString();
                }
                lines_biases[i] += "]";
                lines.Add(lines_biases[i]);
            }

            var tags = GetTag();
            string lines_tags = "tags=";
            for (int i = 0; i < tags.Length; i++) {
                if (!(i == 0))
                    lines_tags += ",";
                lines_tags += tags[i];
            }
            lines.Add(lines_tags);

            return lines.ToArray();
        }
        public void LoadFromFile(string path) {
            if (!File.Exists(path))
                return;

            LoadLines(File.ReadAllLines(path));
        }
        public void LoadLines(string[] lines) {
            List<string> readed = new List<string>();
            readed.AddRange(lines);
            if (readed.Count.Equals(0))
                return;

            if (readed[0].Equals("")) { readed.RemoveAt(0); }

            learning_rate = float.Parse(readed[0].Split('=')[1]);
            readed.RemoveAt(0);

            momentum_rate = float.Parse(readed[0].Split('=')[1]);
            readed.RemoveAt(0);

            error_limit = float.Parse(readed[0].Split('=')[1]);
            readed.RemoveAt(0);

            ignore_error = bool.Parse(readed[0].Split('=')[1]);
            readed.RemoveAt(0);

            infinite_loop = bool.Parse(readed[0].Split('=')[1]);
            readed.RemoveAt(0);

            max_circle = int.Parse(readed[0].Split('=')[1]);
            readed.RemoveAt(0);

            circle = int.Parse(readed[0].Split('=')[1]);
            readed.RemoveAt(0);

            score = float.Parse(readed[0].Split('=')[1]);
            readed.RemoveAt(0);

            fitness = float.Parse(readed[0].Split('=')[1]);
            readed.RemoveAt(0);

            List<int> list = new List<int>();
            int input_layer_len = int.Parse(readed[0].Split('=')[1]);
            list.Add(input_layer_len);
            readed.RemoveAt(0);

            int hidden_layer_len;
            foreach (string len in readed[0].Split('=')[1].Split(' ')) {
                hidden_layer_len = int.Parse(len);
                list.Add(hidden_layer_len);
            }
            readed.RemoveAt(0);

            int output_layer_len = int.Parse(readed[0].Split('=')[1]);
            list.Add(output_layer_len);
            readed.RemoveAt(0);

            int[] layers = list.ToArray();
            this.layers = new Layer[layers.Length];
            for (int j = 0; j < layers.Length; j++) {
                this.layers[j] = new Layer(layers[j], (j != 0), ActivationType.Type.sigmoid, (j == 0 ? PerceptronType.Type.input : (j == layers.Length - 1 ? PerceptronType.Type.output : PerceptronType.Type.hidden)));
            }
            input_layer = this.layers[0];
            hidden_layers = new Layer[layers.Length - 2];
            for (int i = 0; i < hidden_layers.Length; i++) {
                hidden_layers[i] = this.layers[i + 1];
            }
            output_layer = this.layers[this.layers.Length - 1];

            string[] line;
            for (int i = 0; i < this.layers.Length - 1; i++) {
                line = readed[i].Split('=')[1].Replace("[", "").Replace("]", "").Split(' ');
                Synapse[,] synapses = new Synapse[this.layers[i].Length, this.layers[i + 1].Length];
                foreach (string l in line) {
                    int x = int.Parse(l.Split(':')[0].Split(',')[0]);
                    int y = int.Parse(l.Split(':')[0].Split(',')[1]);
                    float w = float.Parse(l.Split(':')[1]);
                    synapses[x, y] = new Synapse(w);
                }

                Perceptron.Connect(this.layers[i].GetPerceptrons(), this.layers[i + 1].GetPerceptrons(), synapses);
            }
            for (int i = 0; i < this.layers.Length - 1; i++) {
                readed.RemoveAt(0);
            }

            for (int i = 0; i < this.layers.Length - 1; i++) {
                line = readed[i].Split('=')[1].Replace("[", "").Replace("]", "").Split(' ');
                int next_len = this.layers[i + 1].Length;

                Synapse[] synapses = new Synapse[next_len];
                for (int y = 0; y < next_len; y++) {
                    synapses[y] = new Synapse(float.Parse(line[y]));
                }
                Perceptron.Connect(this.layers[i].GetBias(), this.layers[i + 1].GetPerceptrons(), synapses);
            }
            for (int i = 0; i < this.layers.Length - 1; i++) {
                readed.RemoveAt(0);
            }

            SetTag(readed[0].Split('=')[1].Split(','));
            readed.RemoveAt(0);
        }
        #endregion

        public int CompareTo(NeuralNetwork other) {
            if (other.Equals(null)) return 1;
            if (fitness > other.fitness) return -1;
            else if (fitness < other.fitness) return 1;
            else return 0;
        }

        public int Length {
            get {
                return layers.Length;
            }
        }
        public Layer this[int index] {
            get { return layers[index]; }          
        }
        
        public void Dispose() {
            GC.SuppressFinalize(this);
        }
    }
}
