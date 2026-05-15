# Shadow Settings Best Practices

## Overview

This guide explains how to properly configure shadow settings in the Netherlands3D Sun package to avoid conflicts, achieve quality results, and optimize performance when integrating with Cesium for Unity.

## Shadow System Architecture

The shadow rendering system has three levels:

```
┌─────────────────────────────────┐
│   UnityEngine.QualitySettings   │  (Global shadow quality settings)
├─────────────────────────────────┤
│   URP Render Pipeline Asset     │  (Active render pipeline settings)
├─────────────────────────────────┤
│   Directional Light Shadow      │  (Per-light shadow settings)
└─────────────────────────────────┘
```

**DynamicShadowDistance manages levels 1 & 2**, ensuring they're synchronized.

## Configuration Levels

### Level 1: Quality Settings (Global)

**Path**: Edit → Project Settings → Quality

```
Shadow Distance:          [depends on DynamicShadowDistance]
Shadow Projection:        Stable Fit (recommended) or Close Fit
Shadow Cascades:          2 or 4
Cascade 2-4 Split:        0.25 (or custom)
Shadow Resolution:        High Resolution, Medium Quality, etc.
```

**Impact**: Applied to all scenes with this quality level

### Level 2: URP Asset Settings (Per-Project)

**Path**: Assets/Settings/YourURPAsset.asset (or find in project)

```
Main Light
├─ Cast Shadows:         ✓ Enabled
├─ Shadow Resolution:    2048 (quality) or 1024 (performance)
├─ Shadow Depth Bias:    1.0 (tune if shadows flicker)
├─ Shadow Normal Bias:   1.0 (reduce for blocky shadows)
└─ Shadow Distance:      [managed by DynamicShadowDistance]

Additional Lights
├─ Cast Shadows:         Disabled (usually, to reduce cost)
└─ ...
```

**Impact**: Applied to all scenes using this URP asset

### Level 3: Light Settings (Per-Light)

**Path**: GameObject with Directional Light component

```
Directional Light
├─ Light
│  ├─ Type:              Directional
│  ├─ Shadows:           Hard or Soft
│  ├─ Shadow Strength:   1.0 (typically)
│  └─ Shadow Bias:       1.0 (light-specific override)
└─ ...
```

**Impact**: Only this specific light instance

## Shadow Distance Scaling Guide

The DynamicShadowDistance component calculates shadow distance based on camera altitude:

```
Shadow Distance = Clamp(
    camera_height * range_multiplier,
    minShadowDistance,
    maxShadowDistance
)
```

### Altitude Scenarios

| Camera Altitude | Typical View | Min Distance | Max Distance | Range Multiplier |
|-----------------|--------------|--------------|--------------|------------------|
| 0-10m | Ground level, buildings | 50m | 500m | 8.0-10.0 |
| 10-100m | City/neighborhood | 100m | 2000m | 6.5-8.0 |
| 100-1000m | Regional, landscape | 200m | 4000m | 5.0-6.5 |
| 1000-10km | Territory, region | 500m | 8000m | 3.0-5.0 |
| 10km+ | Continental, global | 1000m | 16000m | 1.0-3.0 |

### Manual Configuration Example

```csharp
DynamicShadowDistance shadowDistance = GetComponent<DynamicShadowDistance>();

// Ground-level scene (detailed architectural visualization)
shadowDistance.minShadowDistance = 50;
shadowDistance.maxShadowDistance = 500;
shadowDistance.range = 10.0f;

// Mid-altitude scene (city planning, regional view)
shadowDistance.minShadowDistance = 200;
shadowDistance.maxShadowDistance = 4000;
shadowDistance.range = 6.5f;

// High-altitude scene (drone, aircraft view)
shadowDistance.minShadowDistance = 500;
shadowDistance.maxShadowDistance = 10000;
shadowDistance.range = 4.0f;
```

## Preventing Shadow Conflicts

### ✓ DO: Best Practices

1. **Use Single Directional Light for Sun**
   ```csharp
   // Only one light with SunTime should cast shadows
   Light sunLight = GetComponent<Light>();
   sunLight.shadows = LightShadows.Soft;  // or Hard
   sunLight.shadowStrength = 1.0f;
   ```

2. **Disable Shadows on Other Lights**
   ```csharp
   Light artificialLight = GetComponent<Light>();
   artificialLight.shadows = LightShadows.None;  // No shadow cost
   ```

3. **Synchronize Settings Across Levels**
   ```csharp
   // Ensure all three levels agree on shadow settings
   float shadowDist = QualitySettings.shadowDistance;
   Debug.Assert(
       shadowDist == urpAsset.shadowDistance,
       "Shadow distance mismatch between QualitySettings and URP!"
   );
   ```

4. **Configure URP Shadow Cascades Appropriately**
   - **Detailed view (ground level)**: 4 cascades for smooth shadow transitions
   - **Aerial view**: 2 cascades to reduce complexity
   - **Very high view**: 1-2 cascades, larger distances

5. **Adjust Bias for Accuracy**
   ```csharp
   // If shadows look wrong, tune these
   Light sunLight = GetComponent<Light>();
   sunLight.shadowBias = 0.5f;         // Reduce z-fighting
   sunLight.shadowNormalBias = 0.4f;   // Reduce edge artifacts
   ```

### ✗ DON'T: Common Mistakes

1. **Don't Use Multiple Directional Lights with Shadows**
   ```csharp
   // ❌ WRONG - Conflicts with each other
   sunLight.shadows = LightShadows.Soft;
   moonLight.shadows = LightShadows.Soft;

   // ✅ CORRECT
   sunLight.shadows = LightShadows.Soft;
   moonLight.shadows = LightShadows.None;  // Use fake shadows or light probes
   ```

2. **Don't Mix Hard and Soft Shadows**
   ```csharp
   // ❌ Inconsistent - looks bad
   light1.shadows = LightShadows.Hard;
   light2.shadows = LightShadows.Soft;

   // ✅ Consistent
   light1.shadows = LightShadows.Soft;
   light2.shadows = LightShadows.Soft;
   ```

3. **Don't Set Extreme Bias Values**
   ```csharp
   // ❌ Too extreme - causes popping or holes
   light.shadowBias = 10.0f;
   light.shadowNormalBias = 10.0f;

   // ✅ Reasonable range
   light.shadowBias = 0.5f;      // 0.0 - 2.0
   light.shadowNormalBias = 0.4f; // 0.0 - 2.0
   ```

4. **Don't Leave Auto-Detection Unconfigured**
   ```csharp
   // ❌ May not find Cesium in editor or complex scenes
   // Just rely on autoFindCesiumGeoreference

   // ✅ Better - explicit assignment
   sunTime.SetCesiumGeoreference(cesiumGeoreference);
   ```

5. **Don't Ignore Performance**
   ```csharp
   // ❌ Too expensive for mobile
   urpAsset.shadowDistance = 10000;
   shadowResolution = "VeryHigh";
   cascadeCount = 4;

   // ✅ Balanced
   // Adjust based on target platform
   ```

## Platform-Specific Recommendations

### Desktop (High-End)

```
URP Settings:
├─ Shadow Resolution:    4096 or 2048
├─ Cascade Count:        4
├─ Cascade Split:        0.25
└─ Shadow Distance:      4000-8000m (managed by Dynamic)

DynamicShadowDistance:
├─ Min Distance:         100m
├─ Max Distance:         8000m
└─ Range:                6.5
```

**Performance**: 2-5ms per frame

### Laptop/Console

```
URP Settings:
├─ Shadow Resolution:    2048
├─ Cascade Count:        2
├─ Cascade Split:        0.5
└─ Shadow Distance:      2000-4000m

DynamicShadowDistance:
├─ Min Distance:         100m
├─ Max Distance:         4000m
└─ Range:                6.5
```

**Performance**: 1-3ms per frame

### Mobile

```
URP Settings:
├─ Shadow Resolution:    1024 or 512
├─ Cascade Count:        1
├─ Cascade Split:        N/A
└─ Shadow Distance:      500-2000m

DynamicShadowDistance:
├─ Min Distance:         50m
├─ Max Distance:         1000m
└─ Range:                8.0
```

**Performance**: 0.5-2ms per frame

### VR

```
URP Settings:
├─ Shadow Resolution:    1024
├─ Cascade Count:        2
├─ Cascade Split:        0.5
└─ Shadow Distance:      1000-2000m

DynamicShadowDistance:
├─ Min Distance:         100m
├─ Max Distance:         2000m
└─ Range:                6.5
```

**Performance**: < 1.5ms per frame (critical for VR)
**Special Note**: Shadow distance more critical due to headset movement

## Debugging Shadow Issues

### Issue: Shadows Disappear at Distance

**Symptoms**: Objects far from camera don't cast shadows

**Causes**:
1. Shadow Distance too small
2. Camera too high above ground
3. Shadow bias pushing shadows outside range

**Solutions**:
```csharp
// Increase max shadow distance
DynamicShadowDistance shadowDist = GetComponent<DynamicShadowDistance>();
shadowDist.maxShadowDistance = 8000;  // Up from 4000

// Or adjust range multiplier
shadowDist.range = 8.0f;  // Up from 6.5

// Check current value
Debug.Log($"Current shadow distance: {shadowDist.GetCurrentShadowDistance()}");
```

### Issue: Shadows Flicker or Jump

**Symptoms**: Shadow boundaries shift as camera moves

**Causes**:
1. Shadow bias too high
2. Shadow resolution too low
3. Cascade boundaries misaligned

**Solutions**:
```csharp
Light sunLight = GetComponent<Light>();
// Reduce bias values gradually
sunLight.shadowBias = 0.5f;        // Try 0.2-1.0 range
sunLight.shadowNormalBias = 0.4f;  // Try 0.0-1.0 range

// Increase shadow resolution if possible
urpAsset.shadowResolution = UnityEngine.Rendering.Universal.ShadowResolution.High;
```

### Issue: Shadows Look Blocky

**Symptoms**: Shadow edges are jagged or pixelated

**Causes**:
1. Shadow resolution too low
2. Shadow bias obscuring detail
3. Normal bias too high

**Solutions**:
```csharp
// Increase resolution (performance cost)
urpAsset.shadowResolution = UnityEngine.Rendering.Universal.ShadowResolution.VeryHigh;

// Reduce bias
Light sunLight = GetComponent<Light>();
sunLight.shadowNormalBias = 0.1f;  // Reduce from 1.0

// Add shadow soft filtering in URP
urpAsset.supportsMainLightShadows = true;
```

### Issue: Shadows Too Dark or Too Light

**Symptoms**: Overall shadow brightness doesn't match scene

**Causes**:
1. Shadow strength set incorrectly
2. Ambient light too bright/dim
3. Shadow color vs ground color mismatch

**Solutions**:
```csharp
Light sunLight = GetComponent<Light>();
sunLight.shadowStrength = 1.0f;  // 0.0 (invisible) to 1.0 (full)

// Adjust ambient light in scene
RenderSettings.ambientLight = new Color(0.8f, 0.8f, 0.8f);
```

## Cesium-Specific Shadow Considerations

### Working with Cesium Globe

The Cesium globe renders at a massive scale. Shadow settings should account for this:

```csharp
// Cesium scenes are typically at extreme altitude
// The DynamicShadowDistance handles this automatically

// But you can tune for your specific use case:
DynamicShadowDistance shadowDistance = GetComponent<DynamicShadowDistance>();

// For orbital/satellite view
shadowDistance.maxShadowDistance = 16000;
shadowDistance.range = 3.0f;

// For urban/building level detail
shadowDistance.maxShadowDistance = 1000;
shadowDistance.range = 10.0f;
```

### Cesium Terrain Shadows

Cesium terrain is rendered through the standard rendering pipeline:

```
✓ Shadows work normally on Cesium terrain
✓ Sun position affects Cesium terrain lighting
✓ Shadow distance scales with ellipsoid height
✓ No special configuration needed
```

### Multiple Cesium Objects at Different Scales

If you have nested Cesium primitives at different scales:

```csharp
// Set ground level offset to largest scale object
var cesiumGeoreference = FindObjectOfType(
    System.Type.GetType("CesiumForUnity.CesiumGeoreference"));

if (cesiumGeoreference != null)
{
    DynamicShadowDistance shadowDistance = GetComponent<DynamicShadowDistance>();
    
    // Get approximate ellipsoid height
    // (usually available in Cesium component)
    float ellipsoidHeight = 0;  // Query from Cesium
    shadowDistance.SetGroundLevelOffset(ellipsoidHeight);
}
```

## Performance Profiling

### Measure Shadow Rendering Cost

```csharp
using UnityEngine.Profiling;

void Update()
{
    Profiler.BeginSample("Shadow Rendering");
    // Shadow rendering happens here
    Profiler.EndSample();
    
    // Check profiler window for ms cost
}
```

### Optimize Based on Profiling

| Metric | Threshold | Action |
|--------|-----------|--------|
| Shadow time > 5ms | Too expensive | Reduce resolution or cascades |
| Shadow distance > 8000m | Excessive | Reduce maxShadowDistance |
| Memory usage > 200MB | Too high | Reduce shadow atlas resolution |
| FPS < target | Performance issue | Profile and optimize further |

## Quick Reference Checklist

- [ ] Only one Directional Light has shadows enabled
- [ ] Other lights have shadows disabled
- [ ] URP and QualitySettings shadow distances synced
- [ ] DynamicShadowDistance component is active
- [ ] Shadow resolution appropriate for platform
- [ ] Cascade count matches target quality
- [ ] Bias values in reasonable range (0.0 - 2.0)
- [ ] Shadow distance updated dynamically with camera height
- [ ] No flickering or popping artifacts
- [ ] Performance within target (e.g., < 5ms on desktop)

## Additional Resources

- **Unity URP Documentation**: https://docs.unity3d.com/Manual/urp-intro.html
- **Shadow Performance Tips**: https://docs.unity3d.com/Manual/LightPerformance.html
- **Cesium for Unity**: https://github.com/CesiumGS/cesium-unity
- **GeoTimeZone Accuracy**: Automatic timezone handling based on coordinates
