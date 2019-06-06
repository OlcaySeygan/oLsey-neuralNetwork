using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using oLseyLibrary.Model;
using oLseyLibrary.Mathematics;

namespace oLseyLibrary.Genetic {
    public class MeetingPool {
        public static NeuralNetwork ChooseOne(Generation generation, NeuralNetwork other_network = null) {
            float r = RandomF.NextFloat();
            NeuralNetwork network = null;

            if (other_network != null)
                r = RandomF.NextFloat(0f, 1f - other_network.fitness);

            int index = 0;
            for (int i = 0; i < generation.Population; i++) {

                if (other_network != null)
                    if (i == other_network.id) {
                        continue;
                    }
                r -= generation[i].fitness;
                if (r < 0) {
                    index = i;
                    break;
                }
            }
            network = generation[index];
            return network;
        }

        public static NeuralNetwork BestOne(Generation generation) {
            NeuralNetwork network = null;

            int index = 0;
            float high = float.MinValue;
            for (int i = 0; i < generation.Population; i++) {
                if(generation[i].fitness > high) {
                    high = generation[i].fitness;
                    index = i;
                }
            }
            network = generation[index];
            return network;
        }

        public static NeuralNetwork BadOne(Generation generation) {
            NeuralNetwork network = null;

            int index = 0;
            float low = float.MaxValue;
            for (int i = 0; i < generation.Population; i++) {
                if (generation[i].fitness < low) {
                    low = generation[i].fitness;
                    index = i;
                }
            }
            network = generation[index];
            return network;
        }
    }
}
