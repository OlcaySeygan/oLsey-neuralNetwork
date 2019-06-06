namespace oLseyLibrary.Mathematics {
    public class Minumum {
        public static float F(float[] array) {
            float min = float.PositiveInfinity;
            for (int x = 0; x < array.Length; x++) {
                min = array[x] < min ? array[x] : min;
            }
            return min;
        }
        public static float F(float[,] array) {
            float min = float.PositiveInfinity;
            for (int x = 0; x < array.GetLength(0); x++) {
                for (int y = 0; y < array.GetLength(1); y++) {
                    min = array[x, y] < min ? array[x, y] : min;
                }
            }
            return min;
        }
        public static float F(float[,,] array) {
            float min = float.PositiveInfinity;
            for (int x = 0; x < array.GetLength(0); x++) {
                for (int y = 0; y < array.GetLength(1); y++) {
                    for (int z = 0; z < array.GetLength(2); z++) {
                        min = array[x, y, z] < min ? array[x, y, z] : min;
                    }
                }
            }
            return min;
        }
    }
}
