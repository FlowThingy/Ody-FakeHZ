namespace OdyFakeHZ.Core.Models;

/// <summary>
/// Represents a configuration profile for motion smoothness settings.
/// </summary>
public class Profile
{
    /// <summary>
    /// Gets or sets the profile name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the target refresh rate in Hz (60-240).
    /// </summary>
    public int TargetHz { get; set; } = 120;

    /// <summary>
    /// Gets or sets the quality level (0-100, where 0 is fastest/lowest quality, 100 is slowest/highest quality).
    /// </summary>
    public int QualityLevel { get; set; } = 50;

    /// <summary>
    /// Gets or sets the latency tolerance in milliseconds.
    /// </summary>
    public int LatencyMs { get; set; } = 15;

    /// <summary>
    /// Gets or sets whether GPU acceleration is enabled.
    /// </summary>
    public bool UseGpu { get; set; } = true;

    /// <summary>
    /// Gets or sets whether this is a built-in profile (cannot be deleted).
    /// </summary>
    public bool IsBuiltIn { get; set; } = false;

    /// <summary>
    /// Gets or sets the processing mode.
    /// </summary>
    public ProcessingMode ProcessingMode { get; set; } = ProcessingMode.GPU;

    /// <summary>
    /// Creates a Gaming profile preset (low latency, balanced quality).
    /// </summary>
    public static Profile CreateGamingProfile()
    {
        return new Profile
        {
            Name = "Gaming",
            TargetHz = 120,
            QualityLevel = 40,
            LatencyMs = 10,
            UseGpu = true,
            ProcessingMode = ProcessingMode.GPU,
            IsBuiltIn = true
        };
    }

    /// <summary>
    /// Creates a Movies profile preset (higher quality, acceptable latency).
    /// </summary>
    public static Profile CreateMoviesProfile()
    {
        return new Profile
        {
            Name = "Movies",
            TargetHz = 120,
            QualityLevel = 80,
            LatencyMs = 25,
            UseGpu = true,
            ProcessingMode = ProcessingMode.GPU,
            IsBuiltIn = true
        };
    }

    /// <summary>
    /// Creates a Desktop profile preset (balanced settings for productivity).
    /// </summary>
    public static Profile CreateDesktopProfile()
    {
        return new Profile
        {
            Name = "Desktop",
            TargetHz = 90,
            QualityLevel = 60,
            LatencyMs = 15,
            UseGpu = true,
            ProcessingMode = ProcessingMode.Hybrid,
            IsBuiltIn = true
        };
    }
}

/// <summary>
/// Processing modes for frame interpolation.
/// </summary>
public enum ProcessingMode
{
    /// <summary>
    /// GPU-accelerated processing (fastest, requires compatible GPU).
    /// </summary>
    GPU,

    /// <summary>
    /// Hybrid mode that uses GPU when available, falls back to CPU.
    /// </summary>
    Hybrid,

    /// <summary>
    /// CPU-only processing (slower, works on any system).
    /// </summary>
    CPU
}
