# Quick Start Guide

Get the SunCesium package working with Cesium for Unity in 5 minutes.

## Prerequisites

- ✅ Unity 2022.2+
- ✅ Universal Render Pipeline (URP)
- ✅ Cesium for Unity package installed

## 5-Minute Setup

### Step 1: Add the Sun Prefab (1 min)

```
Drag eu.netherlands3d.sun/Runtime/Prefabs/Sun.prefab into your scene
```

### Step 2: Configure the Scene (2 min)

Ensure your scene has:
1. **CesiumGeoreference** (for globe positioning)
2. **CesiumGlobe** (3D terrain)
3. **Main Camera** (viewing the scene)

### Step 3: Play (2 min)

```
Press Play in Unity Editor
```

✅ **Done!** The sun should be positioned based on your Cesium location.

## What Happens Automatically

1. **SunTime** finds your CesiumGeoreference
2. **Extracts** latitude/longitude from it
3. **Calculates** sun position for current date/time
4. **Updates** the Directional Light rotation
5. **DynamicShadowDistance** scales shadows based on camera height

## Customizing Time & Location

### In Inspector

Select the GameObject with SunTime component:

```
Hour:       14              (0-24)
Minutes:    30              (0-60)
Day:        15              (1-31)
Month:      5               (1-12)
Year:       2026            (1-9999)
Animate:    ✓ Enabled       (toggle for real-time progression)
Time Speed: 1               (1 = normal, 60 = fast)
```

### Via Code

```csharp
SunTime sunTime = GetComponent<SunTime>();

// Set time
sunTime.SetTime(14, 30, 0);      // 2:30 PM
sunTime.SetDate(15, 5, 2026);    // May 15, 2026

// Animate
sunTime.ToggleAnimation(true);
sunTime.SetTimeSpeed(60);  // 60x speed

// Manual location (optional, usually auto-detected)
sunTime.SetLocation(longitude: 4.8945f, latitude: 52.3676f);  // Amsterdam
```

## Shadow Basics

The DynamicShadowDistance component automatically adjusts shadows based on how high your camera is:

- **Ground level** → Sharp, detailed shadows
- **High altitude** → Larger shadow area, lower detail
- **Very high** → Capped at max distance to prevent slowdowns

### Adjust Shadow Range

Select any GameObject in your scene and add DynamicShadowDistance:

```
Min Shadow Distance:  100      (closest shadows)
Max Shadow Distance:  4000     (farthest shadows)
Range:                6.5      (height multiplier)
```

For different camera altitudes:
- Ground level buildings: Range = 8.0-10.0
- City overview: Range = 6.5
- Regional view: Range = 5.0

## Troubleshooting

| Problem | Solution |
|---------|----------|
| Sun doesn't move | Check CesiumGeoreference exists in scene |
| Shadows too dark/light | Adjust URP → Main Light Shadows settings |
| Shadows disappear at distance | Increase `Max Shadow Distance` |
| Performance issues | Reduce shadow resolution in URP settings |
| Time doesn't progress | Enable `Animate` checkbox |

## Next Steps

1. **Fine-tune lighting** → See [SHADOW_SETTINGS.md](SHADOW_SETTINGS.md)
2. **Advanced setup** → See [CESIUM_INTEGRATION.md](CESIUM_INTEGRATION.md)
3. **Detailed changes** → See [CHANGES.md](CHANGES.md)
4. **Full documentation** → See [README.md](README.md)

## Common Code Examples

### Example 1: Time-Lapse View

```csharp
public class TimeLapse : MonoBehaviour
{
    public SunTime sunTime;

    void Start()
    {
        sunTime.SetTimeSpeed(600);  // 10 minutes per second
        sunTime.ToggleAnimation(true);
    }
}
```

### Example 2: Real-Time Clock

```csharp
public class RealTimeClock : MonoBehaviour
{
    public SunTime sunTime;

    void Start()
    {
        sunTime.ResetToNow();        // Use system time
        sunTime.ToggleAnimation(true);
    }
}
```

### Example 3: Manual Location Control

```csharp
public class LocationController : MonoBehaviour
{
    public SunTime sunTime;

    public void SetLocation(double lat, double lon)
    {
        sunTime.SetLocation((float)lon, (float)lat);
    }
}
```

## Performance Expectations

| Metric | Value |
|--------|-------|
| Sun update cost | ~0.1ms |
| Shadow update cost | ~0.2-0.5ms |
| Overall impact | < 1ms on desktop |
| Memory usage | < 1MB |

## Need Help?

1. **Check logs** → Look in Unity Console for error messages
2. **Verify setup** → Ensure CesiumGeoreference exists
3. **Read guides** → Check CESIUM_INTEGRATION.md
4. **Debug** → Use GetCurrentShadowDistance() to monitor values

## You're All Set! 🎉

Your scene now has realistic sun positioning and dynamic shadows integrated with Cesium for Unity.

### What You Can Do Now

- ✅ Watch sun move realistically across the sky
- ✅ See shadows change with time of day
- ✅ View at any altitude (ground to space)
- ✅ Animate days, months, or years
- ✅ Change location instantly
- ✅ Optimize for your specific use case

Enjoy your Cesium + Sun integration! 🌞
