# PrimePeter Cesium Sun

Cesium for Unity integration for realistic sun positioning, day/night cycles, and dynamic shadow management.

## Features

- **Dynamic Sun Positioning**: Accurate sun position calculation based on date, time, and Cesium location
- **Realistic Shadows**: Adaptive shadow distance that scales with camera altitude
- **Cesium Integration**: Seamless integration with CesiumGeoreference component
- **Time Control**: Real-time or manual control of time of day
- **Flexible Configuration**: Automatic or manual setup

## Requirements

- Unity 2022.2+
- Universal Render Pipeline (URP)
- **Cesium for Unity package** (required)
- **CesiumGeoreference component** (required)

## Quick Start

1. Add `com.primepeter.cesiumsun` to your project's `Packages` folder
2. Drag the Sun prefab into your Cesium scene
3. Press Play - the sun will automatically use CesiumGeoreference location

## Installation

### Option 1: Direct File
Clone or download into your `Assets/Packages/` folder:
```
Assets/Packages/com.primepeter.cesiumsun/
```

### Option 2: Git Package
In `Packages/manifest.json`, add:
```json
{
  "dependencies": {
    "com.primepeter.cesiumsun": "https://github.com/primepeter/SunCesium.git#main"
  }
}
```

## Usage

### Basic Setup
The package automatically:
1. Detects CesiumGeoreference in your scene
2. Extracts geographic location (latitude/longitude)
3. Calculates sun position for current date/time
4. Adjusts shadows based on camera altitude

### C# API

```csharp
using PrimePeter.CesiumSun;

// Get sun time controller
SunTime sunTime = GetComponent<SunTime>();

// Set time
sunTime.SetTime(14, 30, 0);        // 2:30 PM
sunTime.SetDate(15, 5, 2026);      // May 15, 2026

// Control animation
sunTime.ToggleAnimation(true);
sunTime.SetTimeSpeed(60);          // 60x speed

// Manual location (if needed)
sunTime.SetLocation(longitude: 4.8945f, latitude: 52.3676f);
```

## Documentation

For detailed documentation, see the parent `README.md` in the repository root.

## License

EUPL-1.2 (see LICENSE.txt)

## Credits

- Original sun calculation based on NOAA/USNO algorithms
- Cesium for Unity by Cesium GS
- Inspired by Netherlands3D Sun package
