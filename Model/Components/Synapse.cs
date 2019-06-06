using oLseyLibrary.Mathematics;

namespace oLseyLibrary.Model.Components {
    public class Synapse {
        public float x { get; set; }
        public float error { get; set; }
        public Synapse() {
            this.x = RandomF.NextFloat(-1f, 1f);
            this.error = 0f;
        }
        public Synapse(float value) {
            this.x = value;
            this.error = 0f;
        }
    }
}
