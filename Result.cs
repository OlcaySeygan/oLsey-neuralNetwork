namespace oLseyLibrary {
    public class Result {
        public string tag { get; set; }
        public float value { get; set; }

        public Result(float value, string tag) {
            this.tag = tag;
            this.value = value;
        }
    }
}
