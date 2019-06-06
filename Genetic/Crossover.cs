using oLseyLibrary.Model;
using oLseyLibrary.Mathematics;

namespace oLseyLibrary.Genetic {
    public class Crossover {
        public static NeuralNetwork Child(NeuralNetwork mum, NeuralNetwork dad) {
            NeuralNetwork child = new NeuralNetwork(mum);
            
            for (int i = 0; i < child.Length; i++) {
                for (int j = 0; j < child[i].Length; j++) {
                    for (int k = 0; k < child[i][j].Length; k++) {
                        float value = child[i][j][k].x;

                        int rand = RandomF.Next(2);
                        if (rand == 0) {
                            value = dad[i][j][k].x;
                        } 
                        child[i][j][k].x = value;
                    }
                }
                if (!child[i].IsBiasNull()) {
                    for (int k = 0; k < child[i].GetBias().Length; k++) {
                        float value = child[i].GetBias()[k].x;

                        int rand = RandomF.Next(2);
                        if (rand == 0) {
                            value = dad[i].GetBias()[k].x;
                        }
                        child[i].GetBias()[k].x = value;
                    }
                }
            }

            return child;
        }
    }
}
