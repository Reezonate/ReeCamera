using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace ReeCamera {
    public class SceneLayoutConfig {
        [JsonProperty("IsVisible"), JsonConverter(typeof(ObservableValue<bool>.Converter))]
        public ObservableValue<bool> IsVisibleOV = new ObservableValue<bool>(true);
        
        [JsonProperty("ScreenRect"), JsonConverter(typeof(ObservableValue<Rect>.Converter))]
        public ObservableValue<Rect> ScreenRectOV = new ObservableValue<Rect>(new Rect(0f, 0f, 1f, 1f));

        public MainCameraConfig MainCamera;

        [JsonConverter(typeof(ReeObservableList<SecondaryCameraConfig>.Converter))]
        public ReeObservableList<SecondaryCameraConfig> SecondaryCameras;

        [JsonConstructor]
        public SceneLayoutConfig(int _) {
            MainCamera = MainCameraConfig.Default;
            SecondaryCameras = new ReeObservableList<SecondaryCameraConfig>();
        }

        public SceneLayoutConfig(MainCameraConfig mainCameraConfig, IReadOnlyList<SecondaryCameraConfig> secondaryCameras) {
            MainCamera = mainCameraConfig;
            SecondaryCameras = new ReeObservableList<SecondaryCameraConfig>();
            SecondaryCameras.AddFrom(secondaryCameras);
        }

        public static SceneLayoutConfig Default => new SceneLayoutConfig(0);
    }
}