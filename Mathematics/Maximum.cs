namespace oLseyLibrary.Mathematics {
    public class Maximum {
        public static float F(float[] array) {
            float max = float.NegativeInfinity;
            for (int x = 0; x < array.Length; x++) {
                max = array[x] > max ? array[x] : max;
            }
            return max;
        }
        public static float F(float[,] array) {
            float max = float.NegativeInfinity;
            for (int x = 0; x < array.GetLength(0); x++) {
                for (int y = 0; y < array.GetLength(1); y++) {
                    max = array[x, y] > max ? array[x, y] : max;
                }
            }
            return max;
        }
        public static float F(float[,,] array) {
            float max = float.NegativeInfinity;
            for (int x = 0; x < array.GetLength(0); x++) {
                for (int y = 0; y < array.GetLength(1); y++) {
                    for (int z = 0; z < array.GetLength(2); z++) {
                        max = array[x, y, z] > max ? array[x, y, z] : max;
                    }
                }
            }
            return max;
        }
    }
}
