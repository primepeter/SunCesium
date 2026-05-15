# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2026-05-15

### Added
- Initial release as `com.primepeter.cesiumsun`
- Cesium for Unity integration for sun positioning
- Dynamic shadow distance based on camera altitude
- CesiumIntegration helper component for easy setup
- Comprehensive documentation for Cesium integration
- Shadow settings guide with platform recommendations

### Changed
- Renamed package from `eu.netherlands3d.sun` to `com.primepeter.cesiumsun`
- Updated namespaces to `PrimePeter.CesiumSun`
- Made Cesium-only (removed Netherlands3D Coordinates fallback)
- Simplified and streamlined for Cesium-exclusive projects
- Made a valid Unity Package Manager package

### Removed
- Netherlands3D Coordinates system support
- Fallback coordinate system logic
- External package dependencies (Netherlands3D packages)

### Notes
- This is a Cesium-only fork, not compatible with the original Netherlands3D Sun package
- Requires Cesium for Unity and CesiumGeoreference component
- Version reset to 1.0.0 to reflect the new package identity
