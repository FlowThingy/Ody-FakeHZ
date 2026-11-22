namespace OdyFakeHZ.Engine.Capture;

public class DirectXCaptureManager : IDisposable
{
    private FrameCapture? _frameCapture;
    private bool _isInitialized;

    public bool IsCapturing => _frameCapture?.IsCapturing ?? false;
    public int CapturedFrames => _frameCapture?.CapturedFrameCount ?? 0;
    public double LatencyMs => _frameCapture?.CaptureLatencyMs ?? 0;

    public event EventHandler<CapturedFrame>? FrameCaptured;

    public bool Initialize()
    {
        if (_isInitialized) return true;

        try
        {
            // TODO: Initialize DirectX resources
            // For now, just create the frame capture
            _frameCapture = new FrameCapture();
            _frameCapture.FrameAvailable += OnFrameAvailable;

            _isInitialized = true;
            return true;
        }
        catch
        {
            return false;
        }
    }

    public void StartCapture(int targetFps = 60)
    {
        if (!_isInitialized)
        {
            if (!Initialize())
            {
                throw new InvalidOperationException("Failed to initialize DirectX capture");
            }
        }

        _frameCapture?.StartCapture(targetFps);
    }

    public void StopCapture()
    {
        _frameCapture?.StopCapture();
    }

    private void OnFrameAvailable(object? sender, CapturedFrame frame)
    {
        FrameCaptured?.Invoke(this, frame);
    }

    public void Dispose()
    {
        _frameCapture?.Dispose();
        _isInitialized = false;
    }
}
