using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using oLseyLibrary.Mathematics;
using oLseyLibrary.Model;

namespace oLseyLibrary.Genetic {
    public class Generation {
        private List<NeuralNetwork> neuralNetworks = new List<NeuralNetwork>();

        #region Properties
        private int population;
        public int Population {
            get { return population; }
            set { population = value; }
        }
        private float mutateRate = 0.01f;
        public float MutateRate {
            get { return mutateRate; }
            set { mutateRate = value; }
        }
        private string path;
        public string Path {
            get { return path; }
            set { path = value; }
        }
        private bool saveOnNewGeneration = false;
        public bool SaveOnNewGeneration {
            get { return saveOnNewGeneration; }
            set { saveOnNewGeneration = value; }
        }
        private string pathOfBest;
        public string PathOfBest {
            get { return pathOfBest; }
            set { pathOfBest = value; }
        }
        private bool saveBestNeuralNetwork = false;
        public bool SaveBestNeuralNetwork {
            get { return saveBestNeuralNetwork; }
            set { saveBestNeuralNetwork = value; }
        }
        #endregion
        
        public Generation(int population, params int[] layers) {
            Population = population;
            for (int i = 0; i < population; i++) {
                neuralNetworks.Add(new NeuralNetwork(layers));
                neuralNetworks[i].id = i;
            }
        }
        public Generation(Generation generation) {
            population = generation.population;
            mutateRate = generation.mutateRate;
            path = generation.path;
            saveOnNewGeneration = generation.saveOnNewGeneration;
            pathOfBest = generation.pathOfBest;
            saveBestNeuralNetwork = generation.saveBestNeuralNetwork;
            if (generation.SaveOnNewGeneration) {
                generation.Save();
                if (generation.saveBestNeuralNetwork) {
                    List<NeuralNetwork> sort = new List<NeuralNetwork>();
                    sort.AddRange(generation.neuralNetworks);
                    sort.Sort();
                    sort[0].SaveToFile(generation.pathOfBest);
                }
            }
                
            generation.CalculateFitness();
            
            for (int i = 0; i < population; i++) {
                NeuralNetwork mum = null;
                mum = MeetingPool.ChooseOne(generation);

                NeuralNetwork dad = null;
                dad = MeetingPool.ChooseOne(generation, mum);
                
                NeuralNetwork child = Crossover.Child(mum, dad);
                Mutate.Diseased(child, mutateRate);
                neuralNetworks.Add(child);
            }            
        }

        public void CalculateFitness() {
            float sum = 0f;
            for (int i = 0; i < neuralNetworks.Count; i++) {
                neuralNetworks[i].score = (float)Math.Pow(neuralNetworks[i].score, 2);
                sum += neuralNetworks[i].score;
            }
            for (int i = 0; i < neuralNetworks.Count; i++) {
                if (sum != 0f)
                    neuralNetworks[i].fitness = neuralNetworks[i].score / sum;
                else
                    neuralNetworks[i].fitness = 0f;
            }
        }

        private string brackets = "!-----------------------------------------!";
        public void Save() {
            List<string> lines = new List<string>();

            for (int i = 0; i < population; i++) {
                if (!i.Equals(0)) {
                    lines.Add(brackets);
                }
                lines.AddRange(neuralNetworks[i].SaveLines());
            }

            if (File.Exists(path))
                File.Delete(path);
            File.WriteAllLines(path, lines.ToArray());
        }
        public void Load() {
            if (!File.Exists(path))
                return;

            string text = File.ReadAllText(path);
            string[] separatorBrackets = { brackets };
            string[] separatorNewLine = { "\r\n" };
            string[] split = text.Split(separatorBrackets, StringSplitOptions.None);
            int index;
            for (int i = 0; i < split.Length; i++) {
                index = Map.Int(0, i, split.Length - 1, 0, population - 1);
                List<string> lines = new List<string>();
                lines.AddRange(split[i].Split(separatorNewLine, StringSplitOptions.None));
                neuralNetworks[index].LoadLines(lines.ToArray());
            }
        }
        
        public void Remove(NeuralNetwork neural_network) {
            neuralNetworks.Remove(neural_network);
        }

        public NeuralNetwork this[int index] {
            get { return neuralNetworks[index]; }
        }
    }
}
