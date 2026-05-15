# Cesium for Unity Integration Guide

This guide explains how to integrate the Netherlands3D Sun package with Cesium for Unity to create realistic lighting and shadows for geographic data visualization.

## Prerequisites

- Cesium for Unity package installed and configured
- A scene with a CesiumGlobe and CesiumGeoreference component
- The SunCesium package (eu.netherlands3d.sun) installed

## Setup Process

### Step 1: Create the Cesium Scene Base

If you haven't already:

1. Create a new scene in Unity
2. Install Cesium for Unity (via UPM)
3. Create a CesiumGeoreference component:
   - Right-click in hierarchy → Cesium for Unity → CesiumGeoreference
   - Or use the Cesium quickstart tools

4. Create a Cesium Globe:
   - Right-click in hierarchy → Cesium for Unity → Add Cesium World Terrain
   - This creates the globe geometry and positioning system

### Step 2: Add the Sun

**Option A: Using the Sun Prefab**

1. In your project, navigate to `Packages/eu.netherlands3d.sun/Runtime/Prefabs/`
2. Drag `Sun.prefab` into your scene hierarchy
3. The prefab contains pre-configured `SunTime` and `Directional Light` components

**Option B: Manual Setup**

1. Create a new Directional Light:
   - Right-click hierarchy → Light → Directional Light
   - Position it appropriately (direction is controlled by SunTime)

2. Add the SunTime script:
   - Select the light GameObject
   - Add Component → Netherlands3D.Sun → SunTime

3. Configure SunTime:
   - Assign the Directional Light to "Sun Directional Light" field
   - Set desired date/time
   - Enable/disable animation as needed

### Step 3: Configure Location Integration

The system automatically detects CesiumGeoreference. Verify in the Inspector:

1. **Select the GameObject with SunTime script**
2. **In Inspector, find "Georeference" section**:
   - If "Cesium Georeference" field is empty, ensure CesiumGeoreference exists in your scene
   - Enable "Auto Find Cesium Georeference" (enabled by default)
3. **Click Play** - the sun should now position based on the Cesium location

### Step 4: Configure Dynamic Shadows

For realistic shadow distance at any altitude:

1. **Create or select a GameObject for shadow management**
   - Can be any GameObject, typically the Camera or a Manager object

2. **Add the DynamicShadowDistance script**:
   - Add Component → Netherlands3D.Rendering → DynamicShadowDistance

3. **Configure shadow parameters**:
   - **Range**: 6.5 (default) - adjust based on your altitude range
   - **Min Shadow Distance**: 100 (ground level minimum)
   - **Max Shadow Distance**: 4000 (very high altitude cap)
   - **Reference Transform**: Defaults to main camera (usually correct)
   - **Use Camera Separation From Ground**: Enable if camera height above ellipsoid needs adjustment

### Step 5: Universal Render Pipeline Configuration

Ensure your URP asset is properly configured:

1. **Open your URP asset** (typically in Assets/Settings/)
2. **Verify shadow settings**:
   - ✓ Main Light Shadows: **Enabled**
   - Shadow Resolution: **2048** (for quality scenes) or **1024** (for performance)
   - Cascade Count: **2** or **4** (more cascades = better quality, higher cost)
   - Cascade Split: **0.25** (typical default)

3. **Verify Depth/Normal Pass**:
   - Some features require "Depth & Normal Texture" pass
   - Enable if using advanced shaders

## Advanced Configuration

### Custom Location Fallback

If CesiumGeoreference location detection fails, you can manually set location:

```csharp
SunTime sunTime = GetComponent<SunTime>();
sunTime.SetLocation(longitude: 4.8945f, latitude: 52.3676f);  // Example: Amsterdam
```

### Synchronizing Time Across Network

For multiplayer or collaborative scenes:

```csharp
// Server or authoritative instance sets the time
sunTime.SetTime(14, 30, 0);  // 2:30 PM

// Other clients subscribe to time changes
sunTime.timeOfDayChanged.AddListener((DateTime newTime) => {
    Debug.Log($"Time updated to: {newTime}");
});
```

### Shadow Distance Animation

To smoothly animate shadow distance changes:

```csharp
DynamicShadowDistance shadowDistance = GetComponent<DynamicShadowDistance>();

// Get current shadow distance
float currentDistance = shadowDistance.GetCurrentShadowDistance();

// Set custom ground offset (e.g., for terrain elevation)
shadowDistance.SetGroundLevelOffset(100f);  // 100m above sea level
```

### Multi-Light Scenes

If you need additional lights (e.g., artificial lights, moon):

1. **Create additional light sources**
2. **Disable shadows on secondary lights** to avoid conflicts:
   - Select light → Inspector → Shadows: None
3. **Only the SunTime-controlled light should cast main shadows**

## Debugging

### Verify CesiumGeoreference Connection

Add this debug script temporarily:

```csharp
using UnityEngine;
using Netherlands3D.Sun;

public class DebugCesiumSun : MonoBehaviour
{
    void Start()
    {
        SunTime sunTime = GetComponent<SunTime>();
        
        // Check if location was detected
        Debug.Log($"Sun position: Latitude {}, Longitude {}");
        
        // Check if CesiumGeoreference was found
        var georeference = FindObjectOfType(System.Type.GetType("CesiumForUnity.CesiumGeoreference"));
        Debug.Log($"CesiumGeoreference found: {georeference != null}");
    }
}
```

### Monitor Shadow Distance in Real-Time

Enable Stats in Game view (top-right corner) to monitor:
- Shadow Distance value updates
- Shadow atlas resolution
- Overall render cost

### Common Issues & Solutions

| Issue | Cause | Solution |
|-------|-------|----------|
| Sun doesn't move | CesiumGeoreference not detected | Manually assign in inspector or verify Cesium package installed |
| Shadows disappear at high altitude | Max shadow distance too low | Increase `maxShadowDistance` in DynamicShadowDistance |
| Shadows too blurry close-up | Min shadow distance too high | Decrease `minShadowDistance` or increase shadow resolution |
| Time doesn't change | Animation disabled or time speed = 0 | Check `animate` checkbox and `timeSpeed` value |
| Shadows conflict with other lights | Multiple shadows casting lights | Disable shadows on other directional/spot lights |
| CesiumGeoreference not syncing | Different georeference in scene | Verify single CesiumGeoreference component, multiple will cause conflicts |

## Performance Optimization

### For Mobile/Low-End Devices

```csharp
DynamicShadowDistance shadowDistance = GetComponent<DynamicShadowDistance>();
shadowDistance.minShadowDistance = 50;
shadowDistance.maxShadowDistance = 1000;  // Reduce range
```

Configure URP:
- Shadow Resolution: **512** or **1024**
- Cascade Count: **1**
- Cascade Split: **0.5**

### For High-End Devices

```csharp
// Increase shadow distance range
shadowDistance.maxShadowDistance = 8000;
```

Configure URP:
- Shadow Resolution: **4096**
- Cascade Count: **4**
- Cascade Split: **0.25** (or custom split)

## Examples

### Example 1: Time-Lapse Animation

```csharp
public class TimeLapseSun : MonoBehaviour
{
    public SunTime sunTime;
    public float minutesPerSecond = 60f;

    void Update()
    {
        // Advance time
        float speedMultiplier = minutesPerSecond / 60f;
        sunTime.MultiplyTimeSpeed(speedMultiplier);
    }
}
```

### Example 2: Location-Based Sun

```csharp
public class LocationBasedSun : MonoBehaviour
{
    public SunTime sunTime;
    
    public void SetLocationFromGPS(double latitude, double longitude)
    {
        sunTime.SetLocation((float)longitude, (float)latitude);
        Debug.Log($"Sun position updated to: {latitude}, {longitude}");
    }
}
```

### Example 3: Synchronized Time with UI

```csharp
public class SunTimeUI : MonoBehaviour
{
    public SunTime sunTime;
    public UnityEngine.UI.Text timeDisplay;

    void Start()
    {
        sunTime.timeOfDayChanged.AddListener(OnTimeChanged);
    }

    void OnTimeChanged(System.DateTime newTime)
    {
        timeDisplay.text = newTime.ToString("HH:mm:ss");
    }
}
```

## Cesium Integration Tips

### Camera Height Calculation

In Cesium, camera height above ground is:

```csharp
// Get ellipsoid height from Cesium
var cesiumCamera = GetComponent<CesiumForUnity.CesiumObjectPool>();
float ellipsoidHeight = cesiumCamera.position.y;  // Approximate

// Set as ground offset for accurate shadow distance
DynamicShadowDistance shadowDistance = GetComponent<DynamicShadowDistance>();
shadowDistance.SetGroundLevelOffset(ellipsoidHeight);
```

### Handling Day/Night Cycle at Different Latitudes

The sun calculation automatically handles different latitudes:
- **Near Equator**: Sun path is less extreme
- **High Latitude**: Sun path more extreme, winter darkness
- **Poles**: 6-month day/night cycles

No additional configuration needed - the algorithm handles this.

### Time Zone Handling

The system uses GeoTimeZone library to automatically determine timezone from coordinates:

```csharp
// Time is automatically converted to correct timezone
DateTime localTime = sunTime.Time;  // Already in local timezone
```

## Support & Resources

- **Netherlands3D Documentation**: https://netherlands3d.eu
- **Cesium for Unity**: https://github.com/CesiumGS/cesium-unity
- **Sun Position Algorithm**: Based on NOAA and USNO solar calculations
- **GeoTimeZone**: Accurate timezone lookup by coordinates

## Next Steps

1. ✅ Set up basic Cesium scene
2. ✅ Configure SunTime with CesiumGeoreference
3. ✅ Fine-tune shadow settings for your use case
4. ✅ Add UI controls for time/location adjustment
5. ✅ Optimize for your target platform
6. ✅ Test with different locations and dates
