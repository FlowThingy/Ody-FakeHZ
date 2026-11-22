# Ody-FakeHZ Dependencies

This document lists all external dependencies and libraries used in the Ody-FakeHZ project.

## Core Framework

### .NET 8
- **Version**: 8.0.x
- **Purpose**: Core application framework
- **License**: MIT
- **Website**: https://dot.net/

### Windows App SDK
- **Version**: 1.5.x or later
- **Purpose**: WinUI 3 framework for modern Windows UI
- **License**: MIT
- **NuGet**: `Microsoft.WindowsAppSDK`
- **Website**: https://learn.microsoft.com/windows/apps/windows-app-sdk/

## UI Framework

### WinUI 3
- **Version**: Included with Windows App SDK
- **Purpose**: Modern Windows UI framework with Fluent Design
- **License**: MIT
- **Documentation**: https://learn.microsoft.com/windows/apps/winui/

### CommunityToolkit.WinUI
- **Version**: 8.x
- **Purpose**: Additional UI controls and helpers
- **License**: MIT
- **NuGet**: `CommunityToolkit.WinUI`
- **GitHub**: https://github.com/CommunityToolkit/Windows

### CommunityToolkit.Mvvm
- **Version**: 8.x
- **Purpose**: MVVM patterns and helpers
- **License**: MIT
- **NuGet**: `CommunityToolkit.Mvvm`

### Material.Icons.WinUI
- **Version**: Latest
- **Purpose**: Google Material Design icons
- **License**: Apache 2.0
- **NuGet**: `Material.Icons.WinUI`

## Graphics and Rendering

### Vortice.Windows
- **Version**: 3.x
- **Purpose**: DirectX 12 bindings for .NET
- **License**: MIT
- **NuGet**: `Vortice.Direct3D12`, `Vortice.DXGI`, `Vortice.D3DCompiler`
- **GitHub**: https://github.com/amerkoleci/Vortice.Windows
- **Alternative**: SharpDX (if Vortice compatibility issues arise)

### Windows.Graphics.Capture
- **Version**: Built into Windows SDK
- **Purpose**: Screen capture API
- **License**: Microsoft
- **Documentation**: https://learn.microsoft.com/windows/uwp/audio-video-camera/screen-capture

## Image Processing and Interpolation

### OpenCV.NET (OpenCvSharp4)
- **Version**: 4.x
- **Purpose**: Computer vision and optical flow calculations
- **License**: Apache 2.0
- **NuGet**: `OpenCvSharp4`, `OpenCvSharp4.runtime.win`
- **GitHub**: https://github.com/shimat/opencvsharp
- **Features Used**:
  - Optical flow (Farneback algorithm)
  - GPU acceleration (CUDA/OpenCL)
  - Frame blending and warping

### OpenCvSharp4.Extensions
- **Version**: 4.x
- **Purpose**: Extensions for easier integration
- **License**: Apache 2.0
- **NuGet**: `OpenCvSharp4.Extensions`

## System Integration

### Microsoft.Windows.SDK.Contracts
- **Version**: Latest
- **Purpose**: Windows Runtime APIs
- **License**: Microsoft
- **NuGet**: `Microsoft.Windows.SDK.Contracts`

### PInvoke.User32
- **Version**: 0.7.x
- **Purpose**: P/Invoke wrappers for Windows APIs
- **License**: MIT
- **NuGet**: `PInvoke.User32`
- **GitHub**: https://github.com/dotnet/pinvoke

## Configuration and Serialization

### System.Text.Json
- **Version**: Built into .NET 8
- **Purpose**: JSON serialization for settings and profiles
- **License**: MIT
- **Documentation**: https://learn.microsoft.com/dotnet/api/system.text.json

### Microsoft.Extensions.Configuration
- **Version**: 8.x
- **Purpose**: Configuration management
- **License**: MIT
- **NuGet**: `Microsoft.Extensions.Configuration.Json`

## Logging and Diagnostics

### Microsoft.Extensions.Logging
- **Version**: 8.x
- **Purpose**: Logging framework
- **License**: MIT
- **NuGet**: `Microsoft.Extensions.Logging`

### Serilog
- **Version**: 3.x
- **Purpose**: Structured logging with file rotation
- **License**: Apache 2.0
- **NuGet**: `Serilog`, `Serilog.Sinks.File`, `Serilog.Sinks.Console`
- **GitHub**: https://github.com/serilog/serilog

## Testing

### xUnit
- **Version**: 2.x
- **Purpose**: Unit testing framework
- **License**: Apache 2.0
- **NuGet**: `xunit`, `xunit.runner.visualstudio`
- **Website**: https://xunit.net/

### Moq
- **Version**: 4.x
- **Purpose**: Mocking framework for tests
- **License**: BSD-3-Clause
- **NuGet**: `Moq`
- **GitHub**: https://github.com/moq/moq4

### FluentAssertions
- **Version**: 6.x
- **Purpose**: Fluent assertion library
- **License**: Apache 2.0
- **NuGet**: `FluentAssertions`
- **GitHub**: https://github.com/fluentassertions/fluentassertions

## Packaging and Distribution

### Microsoft MSIX Packaging Tool
- **Version**: Latest
- **Purpose**: Creating Windows installer packages
- **License**: Microsoft
- **Website**: https://learn.microsoft.com/windows/msix/

## Development Tools

### .NET CLI
- **Purpose**: Build, test, and run commands
- **Included**: With .NET SDK

### Git
- **Purpose**: Version control
- **License**: GPL v2
- **Website**: https://git-scm.com/

## Optional Dependencies

### NVIDIA CUDA Toolkit
- **Version**: 11.x or 12.x
- **Purpose**: GPU acceleration on NVIDIA cards
- **License**: NVIDIA
- **Website**: https://developer.nvidia.com/cuda-toolkit
- **Note**: Optional, fallback to OpenCL if unavailable

### AMD ROCm / OpenCL
- **Version**: Latest
- **Purpose**: GPU acceleration on AMD cards
- **License**: MIT / Khronos
- **Website**: https://www.khronos.org/opencl/
- **Note**: Optional, automatic detection

## Installation Commands

### Install .NET templates
```powershell
# Install Windows App SDK templates
dotnet new install Microsoft.WindowsAppSDK.Templates
```

### Restore all NuGet packages
```powershell
dotnet restore
```

### Update all packages
```powershell
dotnet list package --outdated
dotnet add package <PackageName>
```

## License Compliance

All dependencies used in this project are under permissive licenses (MIT, Apache 2.0, BSD) that allow commercial and open-source use. The project itself is licensed under the MIT License.

### License Summary
- **MIT License**: Core framework, most libraries
- **Apache 2.0**: OpenCV, Material Icons
- **BSD-3-Clause**: Moq
- **GPL v2**: Git (development tool, not distributed)
- **Microsoft**: Windows APIs and SDK (system dependencies)

## Security Considerations

- All packages are from official NuGet feeds or verified sources
- Regular updates are recommended for security patches
- OpenCV may require elevated permissions for GPU access
- DWM hooking requires Windows API access (no third-party hooks)

## Performance Notes

- OpenCV with CUDA provides 5-10x speedup over CPU
- DirectX 12 is more efficient than DirectX 11 for our use case
- Vortice.Windows has better performance than legacy SharpDX
- System.Text.Json is faster than Newtonsoft.Json for our needs

## Troubleshooting

### Missing OpenCV DLLs
```powershell
# Reinstall OpenCV runtime
dotnet add package OpenCvSharp4.runtime.win --force
```

### DirectX initialization fails
- Ensure Windows App SDK runtime is installed
- Update GPU drivers
- Check for DirectX 12 support

### GPU acceleration not working
- Verify CUDA/OpenCL installation
- Check GPU driver compatibility
- Fall back to CPU mode in settings

---

**Last Updated**: 2024-01-01 (Task 1 completion)
**Maintained By**: FlowThingy, jellofizz-jpg
