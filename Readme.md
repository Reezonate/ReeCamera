# About
```diff
- This is an early version with bare minimum UI in-game! All config changes are done by manually editing config files
- If you are new to modding or don't like tinkering with your mods, consider using Camera2 mod instead
```

**ReeCamera** is a Beat Saber camera mod for video recording and streaming, featuring:
- Separate presets for the Menu/Gameplay scenes in VR/FPFC
- Masks and true transparency for overlay cameras (for example: back/side swing cameras)
- Support for Spout2 output with multiple sources, allowing to completely bypass screen capture ([Plugin for OBS](github.com/Off-World-Live/obs-spout2-plugin))

![screenshot](Media/screenshot.png)

# How To Use

1) Download the latest mod .zip from the [releases page](https://github.com/Reezonate/ReeCamera/releases)
2) Extract the .zip into your game directory
3) To create main config file, launch and close the game at least once
4) Go to UserData folder and edit your config

## Main Config File `/UserData/ReeCamera.json`
Change **only** when the game is closed, mod will override otherwise
```json
{
  "MainMenuConfigVR": {
    "PresetId": "BSWC Stream.json" // Can be selected in-game
  },
  "GameplayConfigVR": {
    "PresetId": "BSWC Stream.json"
  },
  "MainMenuConfigFPFC": {
    "FramerateSettings": { // Framerate settings for navigating main menu in FPFC
      "VSync": true, // Forces the game to run at your display refresh rate
      "TargetFramerate": 0 // 0 = no fps cap. >0 = fps cap. Does nothing if VSync is on
    },
    "PresetId": "Reesabering.json"
  },
  "GameplayConfigFPFC": {
    "FramerateSettings": { // Framerate settings for watching/recording replays in FPFC
      "VSync": false, // Don't use vsync for video recording, unless your native display refresh rate matches your video framerate
      "TargetFramerate": 60 // Should match your recording settings
    },
    "PresetId": "YT video.json"
  }
}
```

## Scene preset `UserData/ReeCamera/Presets/<filename>.json`
- See built-in presets for more examples
- To refresh press `Shift+Ctrl+F1` or click the `Reload` button
```json
{
  "FormatVersion": 1,
  "ModVersion": "0.0.1",
  "Layouts": [ // List of layouts
    { /* Layout config 1 */ },
    { /* Layout config 2 */ },
    { /* ... */ }
  ] // Note: if you don't use any layouts (like Void preset) screen buffer will stop updating and will become stuck at the last rendered frame in FPFC or will mirror HMD display in VR
}
```

## Layout config:
```json
{
  "IsVisible": true, // Only affects on-screen visibility, doesn't disable cameras and spout output.
  "ScreenRect": { // Position on the screen
    "x": 0.0,
    "y": 0.0,
    "width": 0.5,
    "height": 1.0
  },
  "MainCamera": {
    "SpoutSettings": { // Spout2 lib. Allows to render the game at any resolution directly to a supported app without screen capture. Plugin for OBS: github.com/Off-World-Live/obs-spout2-plugin
      "Enabled": false, // Usecase example: Recording 4k/8k videos without 4k/8k display. Note: screen overlays (like replay controls) are not captured with this method
      "Width": 7680,
      "Height": 4320,
      "ChannelName": "ReeCamera"
    },
    ... // Shared settings
  },
  "SecondaryCameras": [ // Up to 4 cams
    {
      "CompositionSettings": {
        "ScreenRect": { // Position relative to the main
          "x": -0.02,
          "y": -0.02,
          "width": 0.3,
          "height": 0.4
        },
        "MaskTextureId": "SmoothFadeMask.png", // Name of the mask texture file from /UserData/ReeCamera/CustomTextures/
        "Transparent": true, // Use 'true' to make camera background transparent. Make sure to exclude 'Skybox' layer (29) for such cameras 
        "BackgroundColor": {
          "r": 0.8,
          "g": 0.8,
          "b": 0.8,
          "a": 1.0
        },
        "BackgroundBlurLevel": 2, // Background blur amount. Only values from 0 to 5. Very GPU heavy (except 0 ofc)
        "BackgroundBlurScale": 2.0 // Blur radius multiplier
      },
      ... // Shared settings
    }
  ]
}
```

## Shared camera settings 
Used for both Main and Secondary cameras
```json
{
  "Name": "Bottom Left Camera (Back ortho view)",
  "QualitySettings": {
    "AntiAliasing": 1,
    "RenderScale": 1.0
  },
  "CameraSettings": { // Should be self explanatory
    "IgnoreCameraUtils": false,
    "FieldOfView": 60.0,
    "NearClipPlane": 0.3,
    "FarClipPlane": 100.0,
    "Orthographic": true,
    "OrthographicSize": 1.6,
    "CenterOffset": { // View center offset, use values from -1 to 1
      "x": 0.0,
      "y": 0.0
    }
  },
  "MovementConfig": {
    "MovementType": 0, // 0 = static position, 1 = following player's head
    "OffsetType" : 0, // 0 = Global, 1 = Local
    "PositionOffset": {
      "x": 0.0,
      "y": 0.0,
      "z": 0.0 // Increase to move camera backwards
    },
    "RotationOffset": {
      "x": 0.0,
      "y": 0.0,
      "z": 0.0
    },
    "ForceUpright": false, // Remove head tilt
    "PositionalSmoothing": 0.0, // less = more smoothing. But 0 = no smoothing :D
    "RotationalSmoothing": 0.0
  },
  "LayerFilter": {} // Visibility mask. Refer to https://github.com/Reezonate/CameraUtils/blob/master/Source/Core/VisibilityLayer.cs
}
```