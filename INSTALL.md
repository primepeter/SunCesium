# Installation Guide

## For Unity Package Manager (UPM)

SunCesium is a valid Unity Package Manager package. Choose one of the installation methods below:

### Method 1: Git URL (Recommended)

Add to your `Packages/manifest.json`:

```json
{
  "dependencies": {
    "com.primepeter.cesiumsun": "https://github.com/primepeter/SunCesium.git#main"
  }
}
```

### Method 2: Local File Path

1. Clone or download the repository
2. Place it in your project's `Packages/` folder:
   ```
   Packages/com.primepeter.cesiumsun/
   ```

### Method 3: Manual Addition

1. Clone: `git clone https://github.com/primepeter/SunCesium.git`
2. Copy `com.primepeter.cesiumsun/` to your `Packages/` folder
3. Unity will automatically detect and import it

## Verification

After installation:

1. Check `Window → Packages` in Unity
2. You should see "PrimePeter Cesium Sun" in the list
3. Verify the namespace: `using PrimePeter.CesiumSun;`

## Usage

See [QUICKSTART.md](QUICKSTART.md) for 5-minute setup or [CESIUM_INTEGRATION.md](CESIUM_INTEGRATION.md) for detailed integration steps.

## Troubleshooting

### Package Not Showing in Package Manager

- Ensure `package.json` is in the `com.primepeter.cesiumsun/` folder
- Check that the folder name matches the package name exactly: `com.primepeter.cesiumsun`
- Restart Unity if needed

### Namespace Not Found

- Ensure you have `using PrimePeter.CesiumSun;` at the top of your C# files
- Check that the package is correctly imported (should see no compile errors)
- The assembly definition file should be auto-generated in the Scripts folder

### Assembly Definition Issues

- Delete any manually created `.asmdef` files
- Let Unity regenerate the assembly automatically
- The package includes `com.primepeter.cesiumsun.Runtime.asmdef`

## Next Steps

1. ✅ Ensure Cesium for Unity is installed
2. ✅ Add the package via one of the methods above
3. ✅ Follow [QUICKSTART.md](QUICKSTART.md) to get started
4. ✅ Review [CESIUM_INTEGRATION.md](CESIUM_INTEGRATION.md) for advanced setup
