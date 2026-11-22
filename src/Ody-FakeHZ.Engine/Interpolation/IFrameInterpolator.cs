using OdyFakeHZ.Engine.Capture;

namespace OdyFakeHZ.Engine.Interpolation;

public interface IFrameInterpolator
{
    InterpolationQuality Quality { get; set; }
    bool IsInitialized { get; }
    
    bool Initialize();
    InterpolatedFrame[] GenerateFrames(CapturedFrame frame1, CapturedFrame frame2, int targetCount);
    void Dispose();
}

public enum InterpolationQuality
{
    Draft = 0,      // Fast, lower quality
    Balanced = 1,   // Good balance
    High = 2        // Best quality, slower
}

public class InterpolatedFrame
{
    public DateTime Timestamp { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public byte[]? Data { get; set; }
    public double BlendFactor { get; set; }
    public int SourceFrame1 { get; set; }
    public int SourceFrame2 { get; set; }
}
