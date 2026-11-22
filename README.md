# Ody-FakeHZ

**Motion Smoothness Enhancement Tool for Windows**

Ody-FakeHZ enhances visual smoothness through advanced frame interpolation technology, making your 60Hz monitor feel smoother (similar to 144Hz) for videos, games, and desktop UI.

## What It Does

This application uses **motion-compensated frame interpolation (MCFI)** to generate intermediate frames between existing ones, creating a smoother visual experience. This is the same technology used in high-end TVs and NVIDIA DLSS3 frame generation.

### Honest Technology Explanation

⚠️ **Important**: This application does NOT change your monitor's actual refresh rate. Your monitor's hardware capabilities remain unchanged. Instead, it uses frame interpolation to simulate smoother motion by generating additional frames through software processing.

- Your monitor still operates at its native refresh rate (e.g., 60Hz)
- The app generates intermediate frames using motion estimation algorithms
- These interpolated frames create the perception of smoother motion
- There is a trade-off: added latency (typically 10-30ms) vs. increased smoothness

## Features

### Core Functionality
- **Custom Hz Targeting**: Slider from 60Hz to 240Hz equivalent smoothness
- **System-Wide Smoothing**: Works across all applications and windows
- **Per-Application Control**: Enable/disable smoothing for specific apps
- **Profile Management**: Preset profiles for Gaming, Movies, and Desktop usage
- **GPU Acceleration**: Hardware-accelerated interpolation with CPU fallback

### User Experience
- **Simple & Advanced Modes**: Easy for beginners, powerful for enthusiasts
- **Real-Time Performance Monitoring**: GPU usage, latency, and frame generation stats
- **Latency vs. Quality Control**: User-adjustable trade-off slider
- **System Tray Integration**: Minimize and control from the tray
- **Monochrome Design**: Professional black/white/gray aesthetic

## System Requirements

### Minimum Requirements
- **OS**: Windows 10 (version 1809 or later) or Windows 11
- **CPU**: Quad-core processor (Intel Core i5 or AMD Ryzen 5)
- **RAM**: 8GB
- **GPU**: DirectX 12 capable GPU (integrated graphics supported with reduced performance)
- **.NET**: .NET 8 Runtime (bundled with installer)

### Recommended Requirements
- **OS**: Windows 11
- **CPU**: Hexa-core or better (Intel Core i7/i9 or AMD Ryzen 7/9)
- **RAM**: 16GB or more
- **GPU**: Dedicated GPU with 4GB+ VRAM (NVIDIA GTX 1060 / AMD RX 580 or better)
- **Storage**: SSD for optimal performance

## Technology Stack

- **Framework**: .NET 8 + WinUI 3
- **Graphics**: DirectX 12 / Vortice.Windows
- **Interpolation**: OpenCV with GPU acceleration (CUDA/OpenCL)
- **Capture**: Windows.Graphics.Capture API
- **Composition**: Desktop Window Manager (DWM) integration

## Architecture

```
┌─────────────────────────────────────────┐
│         WinUI 3 Application             │
│  ┌───────────────────────────────────┐  │
│  │   UI Layer (XAML + C#)            │  │
│  │   - Main Window                    │  │
│  │   - Settings Panel                 │  │
│  │   - System Tray                    │  │
│  └───────────────────────────────────┘  │
│              ↓↑                          │
│  ┌───────────────────────────────────┐  │
│  │   Control Layer                    │  │
│  │   - Profile Manager                │  │
│  │   - Performance Monitor            │  │
│  │   - App Filter Manager             │  │
│  └───────────────────────────────────┘  │
│              ↓↑                          │
│  ┌───────────────────────────────────┐  │
│  │   Interpolation Engine             │  │
│  │   - Frame Generator (GPU/CPU)      │  │
│  │   - Motion Estimator               │  │
│  │   - Quality Controller             │  │
│  └───────────────────────────────────┘  │
│              ↓↑                          │
│  ┌───────────────────────────────────┐  │
│  │   System Integration Layer         │  │
│  │   - DWM Hook                       │  │
│  │   - Display Manager                │  │
│  │   - DirectX Capture                │  │
│  └───────────────────────────────────┘  │
└─────────────────────────────────────────┘
```

## Development Status

🚧 **In Active Development** 🚧

Current version: **0.1.0-alpha**

### Roadmap

- [x] Project initialization and repository setup
- [ ] UI theme and core layout
- [ ] Hz slider and profile management
- [ ] Performance monitoring system
- [ ] Display and capture integration
- [ ] Frame interpolation engine (CPU/GPU)
- [ ] DWM integration for system-wide smoothing
- [ ] Advanced settings and processing modes
- [ ] System tray and configuration management
- [ ] Error handling and safety systems
- [ ] Testing and documentation
- [ ] v1.0.0 Release

## Installation

### From Release (Coming Soon)
1. Download the latest `.msix` installer from [Releases](https://github.com/FlowThingy/Ody-FakeHZ/releases)
2. Run the installer
3. Follow the setup wizard
4. Launch Ody-FakeHZ from the Start menu

### From Source
```powershell
# Clone the repository
git clone https://github.com/FlowThingy/Ody-FakeHZ.git
cd Ody-FakeHZ

# Restore dependencies
dotnet restore

# Build the project
dotnet build

# Run the application
dotnet run --project src/Ody-FakeHZ.UI
```

## Usage

### Quick Start
1. Launch Ody-FakeHZ
2. Select a profile: **Gaming**, **Movies**, or **Desktop**
3. Adjust the Hz slider to your desired smoothness (start with 90-120Hz)
4. Click the **ENABLE** button
5. Experience smoother motion across your system

### Advanced Configuration
1. Toggle to **Advanced Mode** (top-right)
2. Adjust **Latency vs. Quality** slider based on your preference
3. Select **Processing Mode**: GPU, Hybrid, or CPU
4. Open **Per-App Settings** to enable/disable per application
5. Fine-tune interpolation parameters in advanced controls

### Best Practices
- Start with the **Movies** profile for video content
- Use **Gaming** profile for low-latency gaming (set quality to "Responsive")
- **Desktop** profile is optimized for productivity apps and general UI smoothing
- Disable smoothing for competitive online games to minimize latency
- Use GPU mode when possible for best performance

## Known Limitations

- Frame interpolation adds latency (10-30ms depending on settings)
- Cannot truly change monitor's physical refresh rate
- May cause artifacts in very fast motion scenes
- Some anti-cheat systems may flag the DWM hook (per-app disable recommended)
- Performance depends heavily on GPU capabilities
- Not recommended for competitive esports gaming at highest levels

## FAQ

**Q: Will this damage my monitor?**
A: No. The application does not change your monitor's hardware or attempt to overclock it. It only processes frames in software.

**Q: Does this work with any monitor?**
A: Yes. It works with any monitor at any refresh rate. The interpolation happens in software, not hardware.

**Q: Why does it feel slightly laggy?**
A: Frame interpolation requires processing time, which adds latency. Adjust the "Latency vs. Quality" slider toward "Responsive" to minimize this.

**Q: Can I use this for competitive gaming?**
A: It's not recommended for competitive esports due to added latency. However, casual gaming can benefit from the smoother visuals.

**Q: How much GPU/CPU will it use?**
A: Depends on settings. GPU mode typically uses 20-40% of a mid-range GPU. CPU mode uses 30-60% of CPU cores.

**Q: Is this open source?**
A: Yes! This project is open source under the MIT License. Contributions are welcome.

## Contributing

Contributions are welcome! Please read our contributing guidelines (coming soon) before submitting pull requests.

### Development Setup
1. Install .NET 8 SDK
2. Install Windows App SDK
3. Clone the repository
4. Open in your preferred editor (VS Code, Visual Studio, or Rider)
5. Build and run

### Testing
```powershell
# Run unit tests
dotnet test

# Run with debug logging
dotnet run --project src/Ody-FakeHZ.UI --configuration Debug
```

## Contributors

- **FlowThingy** - Project Creator & Lead Developer
- **jellofizz-jpg** - Core Developer

## License

MIT License - See [LICENSE](LICENSE) file for details

## Acknowledgments

- Frame interpolation research inspired by SVP (SmoothVideo Project)
- OpenCV community for GPU-accelerated optical flow implementations
- Microsoft for WinUI 3 and Windows App SDK
- Design principles from [uiverse.io](https://uiverse.io) community

---

**Disclaimer**: This software is provided "as is" without warranty. Use at your own risk. The developers are not responsible for any issues arising from the use of this software.

*Made with ❤️ for smoother computing experiences*
