namespace oLseyLibrary.Mathematics {
    public class Map {
        public static float Float(float min, float value, float max, float low, float high) {
            if (min.Equals(0f) && max.Equals(0f))
                return 0f;
            return (((value - min) / (max - min)) * (high - low) + low);
        }
        public static int Int(int min, int value, int max, int low, int high) {
            return (int)Float(min, value, max, low, high);
        }
    }
}
