# SunCesium - PrimePeter Cesium Sun

Cesium for Unity integration for realistic sun positioning, day/night cycles, and dynamic shadow management.

**Package Name**: `com.primepeter.cesiumsun`  
**Status**: ✅ Production Ready  
**License**: EUPL-1.2

## Quick Start

### Installation

Add to your `Packages/manifest.json`:

```json
{
  "dependencies": {
    "com.primepeter.cesiumsun": "https://github.com/primepeter/SunCesium.git#main"
  }
}
```

### Usage

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
```

## Features

- ✅ Dynamic sun positioning based on date/time
- ✅ Realistic shadows with adaptive distance
- ✅ Cesium for Unity integration
- ✅ Automatic CesiumGeoreference detection
- ✅ Manual location override support
- ✅ Time control and animation
- ✅ No external dependencies

## Documentation

- **[PACKAGE_README.md](PACKAGE_README.md)** - Package documentation
- **[docs/QUICKSTART.md](docs/QUICKSTART.md)** - 5-minute setup guide
- **[docs/CESIUM_INTEGRATION.md](docs/CESIUM_INTEGRATION.md)** - Detailed integration
- **[docs/SHADOW_SETTINGS.md](docs/SHADOW_SETTINGS.md)** - Shadow configuration
- **[docs/INSTALL.md](docs/INSTALL.md)** - Installation methods
- **[docs/UPM_MIGRATION.md](docs/UPM_MIGRATION.md)** - Migration guide

## Requirements

- Unity 2022.2+
- Universal Render Pipeline (URP)
- **Cesium for Unity** (required)
- **CesiumGeoreference component** (required)

## Project Structure

```
.
├── package.json                    # UPM package manifest
├── PACKAGE_README.md               # Package documentation
├── CHANGELOG.md                    # Version history
├── LICENSE.txt                     # EUPL-1.2 license
├── README.md                       # This file
├── Runtime/
│   ├── Prefabs/
│   │   └── Sun.prefab              # Ready-to-use sun prefab
│   └── Scripts/
│       ├── SunTime.cs              # Sun position controller
│       ├── SunPosition.cs          # Sun calculation math
│       ├── DynamicShadowDistance.cs # Shadow management
│       ├── CesiumIntegration.cs    # Integration helper
│       └── com.primepeter.cesiumsun.Runtime.asmdef
└── docs/                           # Additional documentation
    ├── QUICKSTART.md
    ├── CESIUM_INTEGRATION.md
    ├── SHADOW_SETTINGS.md
    ├── INSTALL.md
    ├── UPM_MIGRATION.md
    └── ...
```

## License

EUPL-1.2 - See LICENSE.txt for details

## Credits

- Original sun calculation based on NOAA/USNO algorithms
- Cesium for Unity by Cesium GS
- Inspired by Netherlands3D Sun package
