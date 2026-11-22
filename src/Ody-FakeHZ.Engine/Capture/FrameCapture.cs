using System.Collections.Concurrent;

namespace OdyFakeHZ.Engine.Capture;

public class FrameCapture : IDisposable
{
    private readonly ConcurrentQueue<CapturedFrame> _frameQueue;
    private bool _isCapturing;
    private Task? _captureTask;
    private readonly object _lock = new();

    public int CapturedFrameCount { get; private set; }
    public double CaptureLatencyMs { get; private set; }
    public bool IsCapturing => _isCapturing;

    public event EventHandler<CapturedFrame>? FrameAvailable;

    public FrameCapture()
    {
        _frameQueue = new ConcurrentQueue<CapturedFrame>();
    }

    public void StartCapture(int targetFps = 60)
    {
        lock (_lock)
        {
            if (_isCapturing) return;

            _isCapturing = true;
            CapturedFrameCount = 0;

            _captureTask = Task.Run(async () => await CaptureLoop(targetFps));
        }
    }

    public void StopCapture()
    {
        lock (_lock)
        {
            _isCapturing = false;
        }

        _captureTask?.Wait();
    }

    private async Task CaptureLoop(int targetFps)
    {
        var frameInterval = TimeSpan.FromMilliseconds(1000.0 / targetFps);
        var lastCaptureTime = DateTime.UtcNow;

        while (_isCapturing)
        {
            var captureStart = DateTime.UtcNow;

            // Simulate frame capture (actual DirectX capture will go here)
            var frame = SimulateCaptureFrame();
            
            var captureEnd = DateTime.UtcNow;
            CaptureLatencyMs = (captureEnd - captureStart).TotalMilliseconds;

            _frameQueue.Enqueue(frame);
            CapturedFrameCount++;

            FrameAvailable?.Invoke(this, frame);

            // Calculate sleep time to maintain target FPS
            var elapsed = DateTime.UtcNow - lastCaptureTime;
            var sleepTime = frameInterval - elapsed;

            if (sleepTime > TimeSpan.Zero)
            {
                await Task.Delay(sleepTime);
            }

            lastCaptureTime = DateTime.UtcNow;
        }
    }

    private CapturedFrame SimulateCaptureFrame()
    {
        // Placeholder - will be replaced with actual DirectX capture
        return new CapturedFrame
        {
            Timestamp = DateTime.UtcNow,
            Width = 1920,
            Height = 1080,
            Data = null // Actual frame data will go here
        };
    }

    public bool TryDequeueFrame(out CapturedFrame? frame)
    {
        return _frameQueue.TryDequeue(out frame);
    }

    public void ClearQueue()
    {
        while (_frameQueue.TryDequeue(out _)) { }
    }

    public void Dispose()
    {
        StopCapture();
        ClearQueue();
    }
}

public class CapturedFrame
{
    public DateTime Timestamp { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public byte[]? Data { get; set; }
    public int FrameNumber { get; set; }
}
