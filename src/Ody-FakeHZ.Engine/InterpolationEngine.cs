using OdyFakeHZ.Engine.Capture;
using OdyFakeHZ.Engine.Interpolation;
using System.Collections.Concurrent;

namespace OdyFakeHZ.Engine;

public class InterpolationEngine : IDisposable
{
    private IFrameInterpolator? _interpolator;
    private readonly ConcurrentQueue<InterpolatedFrame> _outputQueue;
    private bool _isRunning;
    private Task? _processingTask;
    
    private CapturedFrame? _previousFrame;
    private readonly object _lock = new();

    public ProcessingMode Mode { get; private set; }
    public InterpolationQuality Quality { get; set; } = InterpolationQuality.Balanced;
    public int TargetMultiplier { get; set; } = 2; // 60Hz -> 120Hz
    public bool IsRunning => _isRunning;
    
    public int ProcessedFrames { get; private set; }
    public int GeneratedFrames { get; private set; }
    public double InterpolationLatencyMs { get; private set; }

    public event EventHandler<InterpolatedFrame>? FrameGenerated;

    public InterpolationEngine()
    {
        _outputQueue = new ConcurrentQueue<InterpolatedFrame>();
        Mode = ProcessingMode.GPU;
    }

    public bool Initialize(ProcessingMode mode)
    {
        lock (_lock)
        {
            Mode = mode;

            _interpolator = mode switch
            {
                ProcessingMode.CPU => new CpuInterpolator(),
                ProcessingMode.GPU => new GpuInterpolator(),
                ProcessingMode.Hybrid => TryInitializeGpu() ? new GpuInterpolator() : new CpuInterpolator(),
                _ => new CpuInterpolator()
            };

            _interpolator.Quality = Quality;
            return _interpolator.Initialize();
        }
    }

    private bool TryInitializeGpu()
    {
        var gpuInterpolator = new GpuInterpolator();
        if (gpuInterpolator.Initialize())
        {
            return true;
        }
        gpuInterpolator.Dispose();
        return false;
    }

    public void Start()
    {
        lock (_lock)
        {
            if (_isRunning) return;
            if (_interpolator == null || !_interpolator.IsInitialized)
            {
                throw new InvalidOperationException("Engine not initialized");
            }

            _isRunning = true;
            ProcessedFrames = 0;
            GeneratedFrames = 0;
            
            _processingTask = Task.Run(ProcessFrames);
        }
    }

    public void Stop()
    {
        lock (_lock)
        {
            _isRunning = false;
        }

        _processingTask?.Wait();
    }

    public void ProcessCapturedFrame(CapturedFrame frame)
    {
        if (!_isRunning) return;

        lock (_lock)
        {
            if (_previousFrame != null)
            {
                var start = DateTime.UtcNow;
                
                // Generate intermediate frames
                var interpolated = _interpolator!.GenerateFrames(
                    _previousFrame, 
                    frame, 
                    TargetMultiplier - 1
                );

                var end = DateTime.UtcNow;
                InterpolationLatencyMs = (end - start).TotalMilliseconds;

                // Enqueue interpolated frames
                foreach (var interpolatedFrame in interpolated)
                {
                    _outputQueue.Enqueue(interpolatedFrame);
                    GeneratedFrames++;
                    FrameGenerated?.Invoke(this, interpolatedFrame);
                }

                ProcessedFrames++;
            }

            _previousFrame = frame;
        }
    }

    private async Task ProcessFrames()
    {
        while (_isRunning)
        {
            // Processing happens in ProcessCapturedFrame
            // This loop just keeps the task alive
            await Task.Delay(10);
        }
    }

    public bool TryGetFrame(out InterpolatedFrame? frame)
    {
        return _outputQueue.TryDequeue(out frame);
    }

    public void ClearQueue()
    {
        while (_outputQueue.TryDequeue(out _)) { }
    }

    public void Dispose()
    {
        Stop();
        _interpolator?.Dispose();
        ClearQueue();
    }
}

public enum ProcessingMode
{
    CPU,
    GPU,
    Hybrid
}
