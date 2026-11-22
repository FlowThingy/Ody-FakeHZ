using OdyFakeHZ.Engine.Interpolation;

namespace OdyFakeHZ.Engine.Compositor;

public class CompositorManager : IDisposable
{
    private readonly DwmCompositor _dwmCompositor;
    private bool _isRunning;
    private Task? _presentTask;
    private readonly object _lock = new();

    public bool IsInitialized { get; private set; }
    public bool IsRunning => _isRunning;
    public int PresentedFrames { get; private set; }

    public event EventHandler<string>? StatusChanged;

    public CompositorManager()
    {
        _dwmCompositor = new DwmCompositor();
        _dwmCompositor.CompositorReady += (s, e) => StatusChanged?.Invoke(this, "DWM compositor ready");
        _dwmCompositor.Error += (s, msg) => StatusChanged?.Invoke(this, $"Error: {msg}");
    }

    public bool Initialize()
    {
        if (IsInitialized) return true;

        try
        {
            if (!_dwmCompositor.Initialize())
            {
                StatusChanged?.Invoke(this, "Failed to initialize DWM compositor");
                return false;
            }

            if (!_dwmCompositor.Hook())
            {
                StatusChanged?.Invoke(this, "Failed to hook DWM");
                return false;
            }

            IsInitialized = true;
            StatusChanged?.Invoke(this, "Compositor initialized and hooked");
            return true;
        }
        catch (Exception ex)
        {
            StatusChanged?.Invoke(this, $"Initialization failed: {ex.Message}");
            return false;
        }
    }

    public void Start()
    {
        lock (_lock)
        {
            if (_isRunning) return;
            if (!IsInitialized)
            {
                throw new InvalidOperationException("Compositor not initialized");
            }

            _dwmCompositor.Enable();
            _isRunning = true;
            PresentedFrames = 0;

            _presentTask = Task.Run(PresentLoop);
            StatusChanged?.Invoke(this, "Compositor started");
        }
    }

    public void Stop()
    {
        lock (_lock)
        {
            if (!_isRunning) return;

            _isRunning = false;
            _dwmCompositor.Disable();
        }

        _presentTask?.Wait();
        StatusChanged?.Invoke(this, "Compositor stopped");
    }

    public void PresentFrame(InterpolatedFrame frame)
    {
        if (!_isRunning) return;
        
        _dwmCompositor.EnqueueFrame(frame);
    }

    private async Task PresentLoop()
    {
        while (_isRunning)
        {
            if (_dwmCompositor.TryDequeueFrame(out var frame))
            {
                // TODO: Actually present frame to DWM
                // This would send the interpolated frame to the compositor
                
                PresentedFrames++;
            }

            // Wait for next vsync interval
            await Task.Delay(1);
        }
    }

    public void Dispose()
    {
        Stop();
        _dwmCompositor.Dispose();
        IsInitialized = false;
    }
}
