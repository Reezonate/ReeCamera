using UnityEngine;

namespace ReeCamera {
    public readonly struct Range {
        public readonly float Start;
        public readonly float End;
        public readonly float Amplitude;

        public Range(float start, float end) {
            Start = start;
            End = end;
            Amplitude = end - start;
        }

        public float GetRatioClamped(float value) {
            var ratio = GetRatio(value);
            if (ratio <= 0) return 0;
            if (ratio >= 1) return 1;
            return ratio;
        }

        public float GetValueClamped(float ratio) {
            if (ratio <= 0) return Start;
            if (ratio >= 1) return End;
            return GetValue(ratio);
        }

        public float GetRatio(float value) {
            return (value - Start) / Amplitude;
        }

        public float GetRatioSafe(float value) {
            return Amplitude == 0 ? 0.0f : (value - Start) / Amplitude;
        }

        public float GetValue(float ratio) {
            return Start + Amplitude * ratio;
        }

        public bool ContainsValue(float value) {
            if (Mathf.Approximately(value, Start)) return true;
            if (Mathf.Approximately(value, End)) return true;
            
            if (Start < End) {
                return value >= Start && value <= End;
            }

            return value <= Start && value >= End;
        }
    }
}