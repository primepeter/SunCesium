# SunCesium

Enhanced Netherlands3D Sun package with Cesium for Unity integration. This package enables realistic day/night cycles and shadow simulation for Cesium-based scenes, allowing dynamic sun positioning based on geographic location and time.

## Features

- **Dynamic Sun Positioning**: Accurate sun position calculation based on date, time, and geographic coordinates
- **Realistic Shadows**: Adaptive shadow distance that scales with camera altitude
- **Cesium Integration**: Seamless integration with Cesium for Unity's CesiumGeoreference component
- **Backward Compatible**: Maintains compatibility with Netherlands3D Coordinates system
- **Flexible Configuration**: Support for both automatic and manual location setup

## Installation

### Prerequisites

- Unity 2022.2 or later
- Universal Render Pipeline (URP)
- Cesium for Unity (optional, but recommended for globe rendering)

### Setup Steps

1. **Add the package** to your Unity project:
   - Option A: Clone this repository into your `Assets/Packages/` folder
   - Option B: Add via OpenUPM: `openupm add eu.netherlands3d.sun`

2. **Ensure dependencies** are installed:
   - `eu.netherlands3d.coordinates`
   - `eu.netherlands3d.geotimezone`

## Usage

### Quick Start with Cesium

1. **Create a Sun in your scene**:
   - Drag the `Sun.prefab` from `Runtime/Prefabs/` into your scene
   - Or manually create a Directional Light component

2. **Set up the Sun Time Controller**:
   - Add the `SunTime` script to a GameObject in your scene
   - Assign your Directional Light to the "Sun Directional Light" field
   - The script will automatically detect CesiumGeoreference if present

3. **Configure Shadow Distance** (Optional):
   - Add the `DynamicShadowDistance` script to any GameObject
   - It will automatically reference your main camera
   - Adjust `min/max shadow distance` based on your scene scale

4. **Enable Cesium Integration**:
   - Add the `CesiumIntegration` helper script to auto-connect components
   - Or manually assign CesiumGeoreference via the `SetCesiumGeoreference()` method

### Manual Configuration

If you need manual control over location:

```csharp
sunTime.SetLocation(longitude, latitude);
sunTime.SetTime(hour, minute, second);
sunTime.SetDate(day, month, year);
```

### Using Netherlands3D Coordinates (Legacy)

If you're not using Cesium, the package falls back to the Netherlands3D Coordinates system:

```csharp
// Configure the Coordinates package origin, then:
sunTime.RecalculateOrigin(); // Updates from CoordinateSystems.CoordinateAtUnityOrigin
```

## Configuration

### SunTime Script

| Property | Description |
|----------|-------------|
| **Hour/Minutes/Seconds** | Current time of day |
| **Day/Month/Year** | Current date |
| **Sun Directional Light** | Reference to the light source representing the sun |
| **Animate** | Enable/disable real-time time progression |
| **Time Speed** | Multiplier for time progression (1 = normal speed) |
| **CesiumGeoreference** | Reference to the CesiumGeoreference component |
| **Auto Find Cesium Georeference** | Automatically locate CesiumGeoreference if not assigned |

### DynamicShadowDistance Script

| Property | Description |
|----------|-------------|
| **Range** | Multiplier for shadow distance based on height (default: 6.5) |
| **Min Shadow Distance** | Minimum shadow distance (default: 100m) |
| **Max Shadow Distance** | Maximum shadow distance (default: 4000m) |
| **Reference Transform** | Camera or point for height calculation (auto-uses main camera) |
| **Use Camera Separation From Ground** | Account for camera height above terrain/globe |
| **Ground Level Offset** | Height offset for terrain/ellipsoid base |

## Shadow Settings Guide

The DynamicShadowDistance system ensures shadows look correct at all altitudes:

- **Low Altitude** (ground level): Uses minShadowDistance for sharp, detailed shadows
- **High Altitude** (above landscape): Scales shadow distance proportionally to camera height
- **Very High Altitude** (aircraft/satellite view): Capped at maxShadowDistance to prevent performance issues

### Recommended Settings for Cesium Scenes

| Scenario | Min Distance | Max Distance | Range |
|----------|-------------|-------------|-------|
| Urban detail | 50m | 2000m | 8.0 |
| Regional view | 100m | 4000m | 6.5 |
| Continent scale | 500m | 10000m | 5.0 |

## Shadow Conflict Prevention

To prevent shadow issues when combining with other lighting systems:

1. **Only use one sun**: Ensure only one Directional Light with the SunTime script is active
2. **Disable other shadow-casting lights**: Set other lights to "Bake" mode or disable shadows
3. **Configure URP correctly**:
   - Set Main Light Shadows to enabled
   - Adjust Shadow Resolution and Cascades based on performance
   - Ensure Shadow Distance matches DynamicShadowDistance settings

## API Reference

### SunTime Methods

```csharp
// Location control
void SetLocation(float longitude, float latitude);
void SetCesiumGeoreference(Component georeference);
void RecalculateOrigin();

// Time control
void SetTime(int hour, int minute, int second);
void SetDate(int day, int month, int year);
void SetTime(DateTime time);
void ResetToNow();

// Animation control
void ToggleAnimation(bool animate);
void SetTimeSpeed(float speed);
void MultiplyTimeSpeed(float factor);
```

### DynamicShadowDistance Methods

```csharp
void SetGroundLevelOffset(float offset);
float GetCurrentShadowDistance();
```

## Events

### SunTime Events

```csharp
UnityEvent<DateTime> timeOfDayChanged;    // Fired when time changes
UnityEvent<float> timeSpeedChanged;       // Fired when time speed changes
UnityEvent<bool> useCurrentTimeChanged;   // Fired when current time mode changes
UnityEvent<bool> isAnimatingChanged;      // Fired when animation state changes
```

## Troubleshooting

### CesiumGeoreference Not Detected

- Ensure CesiumGeoreference component is in the active scene
- Check that Cesium for Unity package is properly installed
- Manually assign CesiumGeoreference via inspector or `SetCesiumGeoreference()` method

### Shadows Look Wrong

1. Check DynamicShadowDistance is active in the scene
2. Verify camera reference is correct
3. Check URP Shadow Cascade settings
4. Ensure shadow resolution is appropriate for your scene scale
5. Verify no other shadow-casting lights are conflicting

### Sun Position Incorrect

1. Verify location (latitude/longitude) is correct
2. Check system date/time settings
3. Ensure timezone is correct (GeoTimeZone handles this automatically)
4. Verify CesiumGeoreference position matches intended location

### Performance Issues

1. Reduce `max shadow distance`
2. Decrease shadow map resolution in URP settings
3. Reduce number of shadow cascades
4. Lower overall shadow quality settings

## License

This package is provided under the EUPL-1.2 license. See LICENSE.txt for details.

## Credits

- **Original Sun Calculation**: Based on algorithms from [astro.uio.no](http://www.astro.uio.no/~bgranslo/aares/calculate.html)
- **Netherlands3D**: Core sun and coordinates packages
- **Cesium.js**: Geospatial web mapping foundation


