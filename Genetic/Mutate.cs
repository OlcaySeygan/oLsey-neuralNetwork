using oLseyLibrary.Mathematics;
using oLseyLibrary.Model;

namespace oLseyLibrary.Genetic {
    public class Mutate {
        public static NeuralNetwork Diseased(NeuralNetwork neuralNetwork, float rate) {
            for (int i = 0; i < neuralNetwork.Length; i++) {
                for (int j = 0; j < neuralNetwork[i].Length; j++) {
                    for (int k = 0; k < neuralNetwork[i][j].Length; k++) {
                        if(RandomF.NextFloat() < rate) {
                            float value = neuralNetwork[i][j][k].x;
                            var offset = RandomF.NextGaussian() * 0.5f;
                            value += offset;
                            neuralNetwork[i][j][k].x = value;
                        }
                    }
                }
                if (!neuralNetwork[i].IsBiasNull()) {
                    for (int k = 0; k < neuralNetwork[i].GetBias().Length; k++) {
                        if (RandomF.NextFloat() < rate) {
                            float value = neuralNetwork[i].GetBias()[k].x;
                            var offset = RandomF.NextGaussian() * 0.5f;
                            value += offset;
                            neuralNetwork[i].GetBias()[k].x = value;
                        }
                    }
                }
            }
            return new NeuralNetwork(neuralNetwork);
        }
    }
}
