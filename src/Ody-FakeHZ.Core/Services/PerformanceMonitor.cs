using System.Diagnostics;

namespace OdyFakeHZ.Core.Services;

public class PerformanceMonitor
{
    private readonly PerformanceCounter? _cpuCounter;
    private readonly PerformanceCounter? _ramCounter;
    private DateTime _lastFrameTime;
    private int _frameCount;
    private double _currentFps;
    private double _latencyMs;

    public double GpuUsage { get; private set; }
    public double CpuUsage { get; private set; }
    public double RamUsage { get; private set; }
    public double LatencyMs => _latencyMs;
    public double CurrentFps => _currentFps;
    public bool IsActive { get; set; }

    public event EventHandler<PerformanceData>? MetricsUpdated;

    public PerformanceMonitor()
    {
        try
        {
            _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            _ramCounter = new PerformanceCounter("Memory", "% Committed Bytes In Use");
        }
        catch
        {
            // Performance counters might not be available
        }

        _lastFrameTime = DateTime.UtcNow;
    }

    public void StartMonitoring()
    {
        IsActive = true;
        Task.Run(async () => await MonitorLoop());
    }

    public void StopMonitoring()
    {
        IsActive = false;
    }

    private async Task MonitorLoop()
    {
        while (IsActive)
        {
            UpdateMetrics();
            
            var data = new PerformanceData
            {
                GpuUsage = GpuUsage,
                CpuUsage = CpuUsage,
                RamUsage = RamUsage,
                LatencyMs = LatencyMs,
                Fps = CurrentFps,
                Timestamp = DateTime.UtcNow
            };

            MetricsUpdated?.Invoke(this, data);

            await Task.Delay(100); // Update every 100ms
        }
    }

    private void UpdateMetrics()
    {
        // CPU usage
        try
        {
            CpuUsage = _cpuCounter?.NextValue() ?? 0;
        }
        catch
        {
            CpuUsage = 0;
        }

        // RAM usage
        try
        {
            RamUsage = _ramCounter?.NextValue() ?? 0;
        }
        catch
        {
            RamUsage = 0;
        }

        // GPU usage (placeholder - will be implemented with DirectX integration)
        GpuUsage = IsActive ? Random.Shared.Next(20, 45) : 0;

        // Latency calculation (simulated for now)
        _latencyMs = IsActive ? 8.0 + Random.Shared.NextDouble() * 4.0 : 0;

        // FPS tracking
        var now = DateTime.UtcNow;
        var elapsed = (now - _lastFrameTime).TotalSeconds;
        
        if (elapsed >= 1.0)
        {
            _currentFps = _frameCount / elapsed;
            _frameCount = 0;
            _lastFrameTime = now;
        }
    }

    public void RecordFrame()
    {
        _frameCount++;
    }

    public void SetLatency(double latencyMs)
    {
        _latencyMs = latencyMs;
    }
}

public class PerformanceData
{
    public double GpuUsage { get; set; }
    public double CpuUsage { get; set; }
    public double RamUsage { get; set; }
    public double LatencyMs { get; set; }
    public double Fps { get; set; }
    public DateTime Timestamp { get; set; }
}
