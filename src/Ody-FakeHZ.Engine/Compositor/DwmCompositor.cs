using OdyFakeHZ.Engine.Interpolation;
using System.Runtime.InteropServices;

namespace OdyFakeHZ.Engine.Compositor;

public class DwmCompositor : IDisposable
{
    private bool _isHooked;
    private bool _isEnabled;
    private readonly Queue<InterpolatedFrame> _frameQueue;
    private readonly object _lock = new();

    public bool IsHooked => _isHooked;
    public bool IsEnabled => _isEnabled;
    public int QueuedFrames => _frameQueue.Count;

    public event EventHandler? CompositorReady;
    public event EventHandler<string>? Error;

    [DllImport("dwmapi.dll")]
    private static extern int DwmIsCompositionEnabled(out bool enabled);

    public DwmCompositor()
    {
        _frameQueue = new Queue<InterpolatedFrame>();
    }

    public bool Initialize()
    {
        try
        {
            // Check if DWM composition is enabled
            if (DwmIsCompositionEnabled(out bool compositionEnabled) != 0)
            {
                Error?.Invoke(this, "Failed to check DWM composition status");
                return false;
            }

            if (!compositionEnabled)
            {
                Error?.Invoke(this, "DWM composition is not enabled");
                return false;
            }

            // TODO: Set up actual DWM hooks
            // For now, just mark as initialized
            
            CompositorReady?.Invoke(this, EventArgs.Empty);
            return true;
        }
        catch (Exception ex)
        {
            Error?.Invoke(this, $"DWM initialization failed: {ex.Message}");
            return false;
        }
    }

    public bool Hook()
    {
        lock (_lock)
        {
            if (_isHooked) return true;

            try
            {
                // TODO: Implement actual DWM hooking
                // This would involve:
                // 1. Getting DWM's rendering pipeline
                // 2. Injecting our frame presentation callback
                // 3. Setting up frame timing synchronization
                
                // For now, simulate successful hook
                _isHooked = true;
                return true;
            }
            catch (Exception ex)
            {
                Error?.Invoke(this, $"Hook failed: {ex.Message}");
                return false;
            }
        }
    }

    public void Unhook()
    {
        lock (_lock)
        {
            if (!_isHooked) return;

            try
            {
                // TODO: Clean up DWM hooks
                
                _isHooked = false;
                _frameQueue.Clear();
            }
            catch (Exception ex)
            {
                Error?.Invoke(this, $"Unhook failed: {ex.Message}");
            }
        }
    }

    public void Enable()
    {
        lock (_lock)
        {
            if (!_isHooked)
            {
                throw new InvalidOperationException("Must hook before enabling");
            }

            _isEnabled = true;
        }
    }

    public void Disable()
    {
        lock (_lock)
        {
            _isEnabled = false;
            _frameQueue.Clear();
        }
    }

    public void EnqueueFrame(InterpolatedFrame frame)
    {
        lock (_lock)
        {
            if (!_isEnabled) return;

            _frameQueue.Enqueue(frame);

            // Keep queue size reasonable
            while (_frameQueue.Count > 30)
            {
                _frameQueue.Dequeue();
            }
        }
    }

    public bool TryDequeueFrame(out InterpolatedFrame? frame)
    {
        lock (_lock)
        {
            if (_frameQueue.Count > 0)
            {
                frame = _frameQueue.Dequeue();
                return true;
            }

            frame = null;
            return false;
        }
    }

    public void Dispose()
    {
        Disable();
        Unhook();
    }
}
