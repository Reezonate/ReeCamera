using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace ReeCamera {
    [CreateAssetMenu(fileName = "CompositionManager", menuName = "Composition")]
    public class CompositionManagerSO : ScriptableObject {
        #region Properties

        private struct CompositionProperties {
            public int MainTexPropertyID;
            public int MaskTexPropertyID;
            public int ScreenRectPropertyID;
        }

        private const int MaxCameras = 4;

        private static readonly int ScreenRectPropertyId = Shader.PropertyToID("_ScreenRect");
        private static readonly int BackgroundColorPropertyId = Shader.PropertyToID("_BackgroundColor");
        private static readonly int BlurScalePropertyId = Shader.PropertyToID("_BlurScale");

        private static readonly CompositionProperties[] Properties = new CompositionProperties[MaxCameras];
        private static readonly string[] CamCountKeywords = new string[MaxCameras];

        static CompositionManagerSO() {
            for (var i = 0; i < Properties.Length; i++) {
                CompositionProperties tmp;
                tmp.MainTexPropertyID = Shader.PropertyToID($"_Cam{i}Tex");
                tmp.MaskTexPropertyID = Shader.PropertyToID($"_Cam{i}MaskTex");
                tmp.ScreenRectPropertyID = Shader.PropertyToID($"_Cam{i}Rect");
                Properties[i] = tmp;

                CamCountKeywords[i] = $"CAM_COUNT_{i + 1}";
            }
        }

        #endregion

        #region Serialized

        public Material clearMaterialBase;
        public Material compositionMaterialBase;
        public Mesh postProcessQuad;

        #endregion

        #region CreateComposition

        public Composition CreateComposition(
            IEnumerable<OverlayCamera> cameras,
            int screenWidth,
            int screenHeight,
            int antiAliasing
        ) {
            var compositionCameras = new List<CompositionCamera>();

            foreach (var overlayCamera in cameras) {
                if (compositionCameras.Count >= MaxCameras) break;
                compositionCameras.Add(CreateCompositionCamera(overlayCamera, screenWidth, screenHeight, antiAliasing));
            }

            var compositionMaterial = CreateCompositionMaterial(compositionCameras);
            return new Composition(compositionCameras, compositionMaterial, screenWidth, screenHeight);
        }

        private CompositionCamera CreateCompositionCamera(in OverlayCamera overlayCamera, int screenWidth, int screenHeight, int antiAliasing) {
            AdjustRectangle(
                overlayCamera.ScreenRect, screenWidth, screenHeight,
                out var pixelAdjustedRect, out var textureWidth, out var textureHeight
            );

            var camera = overlayCamera.Camera;
            var targetTexture = CreateCameraTargetTexture(textureWidth, textureHeight, antiAliasing);
            var clearMaterial = CreateClearMaterial(pixelAdjustedRect, overlayCamera.BackgroundColor, overlayCamera.BackgroundBlurLevel, overlayCamera.BackgroundBlurScale);

            camera.enabled = false;
            camera.targetTexture = targetTexture;
            camera.clearFlags = CameraClearFlags.SolidColor;

            CommandBuffer commandBuffer;
            if (overlayCamera.IsTransparent) {
                commandBuffer = CreateClearCommandBuffer(clearMaterial);
                camera.backgroundColor = Color.clear;
                camera.AddCommandBuffer(CameraEvent.BeforeForwardOpaque, commandBuffer);
            } else {
                var col = overlayCamera.BackgroundColor;
                col.a = 0;
                camera.backgroundColor = col;
                commandBuffer = null;
            }

            return new CompositionCamera {
                Camera = camera,
                ScreenRect = pixelAdjustedRect,
                MaskTexture = overlayCamera.MaskTexture,
                TargetTexture = targetTexture,
                ClearMaterial = clearMaterial,
                ClearCommandBuffer = commandBuffer
            };
        }

        #endregion

        #region Utils

        private Material CreateCompositionMaterial(IReadOnlyList<CompositionCamera> cameras) {
            var compositionMaterial = Instantiate(compositionMaterialBase);

            compositionMaterial.EnableKeyword(CamCountKeywords[cameras.Count - 1]);

            for (var i = 0; i < cameras.Count; i++) {
                var properties = Properties[i];
                var camera = cameras[i];

                compositionMaterial.SetTexture(properties.MainTexPropertyID, camera.TargetTexture);
                compositionMaterial.SetTexture(properties.MaskTexPropertyID, camera.MaskTexture);
                compositionMaterial.SetVector(properties.ScreenRectPropertyID, camera.ScreenRect);
            }

            return compositionMaterial;
        }

        private Material CreateClearMaterial(in Vector4 screenRectangle, in Color backgroundColor, int blurLevel, float blurScale) {
            blurLevel = Mathf.Clamp(blurLevel, 0, 5);

            var material = Instantiate(clearMaterialBase);
            material.EnableKeyword($"BLUR_LEVEL_{blurLevel}");
            material.SetVector(ScreenRectPropertyId, screenRectangle);
            material.SetColor(BackgroundColorPropertyId, backgroundColor);
            material.SetFloat(BlurScalePropertyId, blurScale);
            return material;
        }

        private static RenderTexture CreateCameraTargetTexture(int textureWidth, int textureHeight, int antiAliasing) {
            var texture = new RenderTexture(textureWidth, textureHeight, 32, RenderTextureFormat.ARGB32) {
                antiAliasing = antiAliasing
            };
            texture.Create();
            return texture;
        }

        private CommandBuffer CreateClearCommandBuffer(Material clearMaterialInstance) {
            var commandBuffer = new CommandBuffer();
            commandBuffer.DrawMesh(postProcessQuad, Matrix4x4.identity, clearMaterialInstance);
            return commandBuffer;
        }

        private static void AdjustRectangle(
            Vector4 rawRect, int screenWidth, int screenHeight,
            out Vector4 pixelAdjustedRect, out int textureWidth, out int textureHeight
        ) {
            var x = Mathf.RoundToInt(screenWidth * rawRect.x);
            var y = Mathf.RoundToInt(screenHeight * rawRect.y);
            textureWidth = Mathf.RoundToInt(screenWidth * rawRect.z);
            textureHeight = Mathf.RoundToInt(screenHeight * rawRect.w);
            pixelAdjustedRect = new Vector4(
                (float)x / screenWidth,
                (float)y / screenHeight,
                (float)textureWidth / screenWidth,
                (float)textureHeight / screenHeight
            );
        }

        #endregion
    }
}