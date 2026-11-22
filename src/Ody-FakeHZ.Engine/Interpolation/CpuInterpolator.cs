using OdyFakeHZ.Engine.Capture;

namespace OdyFakeHZ.Engine.Interpolation;

public class CpuInterpolator : IFrameInterpolator
{
    public InterpolationQuality Quality { get; set; } = InterpolationQuality.Balanced;
    public bool IsInitialized { get; private set; }

    public bool Initialize()
    {
        IsInitialized = true;
        return true;
    }

    public InterpolatedFrame[] GenerateFrames(CapturedFrame frame1, CapturedFrame frame2, int targetCount)
    {
        if (!IsInitialized)
            throw new InvalidOperationException("Interpolator not initialized");

        if (targetCount <= 0)
            return Array.Empty<InterpolatedFrame>();

        var frames = new InterpolatedFrame[targetCount];
        var timeDiff = frame2.Timestamp - frame1.Timestamp;

        for (int i = 0; i < targetCount; i++)
        {
            double t = (i + 1.0) / (targetCount + 1.0); // Blend factor between 0 and 1
            
            frames[i] = new InterpolatedFrame
            {
                Timestamp = frame1.Timestamp + TimeSpan.FromTicks((long)(timeDiff.Ticks * t)),
                Width = frame1.Width,
                Height = frame1.Height,
                BlendFactor = t,
                SourceFrame1 = frame1.FrameNumber,
                SourceFrame2 = frame2.FrameNumber,
                Data = BlendFrames(frame1.Data, frame2.Data, t)
            };
        }

        return frames;
    }

    private byte[]? BlendFrames(byte[]? data1, byte[]? data2, double t)
    {
        // Simple linear blend for now
        // In real implementation, this would do motion-compensated blending
        
        if (data1 == null || data2 == null)
            return null;

        if (data1.Length != data2.Length)
            return null;

        var result = new byte[data1.Length];
        
        switch (Quality)
        {
            case InterpolationQuality.Draft:
                // Simple copy of nearest frame
                Array.Copy(t < 0.5 ? data1 : data2, result, data1.Length);
                break;
                
            case InterpolationQuality.Balanced:
            case InterpolationQuality.High:
                // Linear interpolation
                for (int i = 0; i < data1.Length; i++)
                {
                    result[i] = (byte)(data1[i] * (1 - t) + data2[i] * t);
                }
                break;
        }

        return result;
    }

    public void Dispose()
    {
        IsInitialized = false;
    }
}
