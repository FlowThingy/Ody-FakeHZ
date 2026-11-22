using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using OdyFakeHZ.Core.Services;
using OdyFakeHZ.Core.Models;
using OdyFakeHZ.Engine.Capture;

namespace OdyFakeHZ.UI;

public sealed partial class MainWindow : Window
{
    private bool _isEnabled = false;
    private readonly ProfileManager _profileManager;
    private readonly PerformanceMonitor _performanceMonitor;
    private readonly DisplayManager _displayManager;
    private readonly DirectXCaptureManager _captureManager;
    private Microsoft.UI.Dispatching.DispatcherQueue? _dispatcherQueue;

    public MainWindow()
    {
        this.InitializeComponent();
        
        _profileManager = new ProfileManager();
        _profileManager.ProfileChanged += OnProfileChanged;
        
        _performanceMonitor = new PerformanceMonitor();
        _performanceMonitor.MetricsUpdated += OnMetricsUpdated;

        _displayManager = new DisplayManager();
        _captureManager = new DirectXCaptureManager();
        _captureManager.FrameCaptured += OnFrameCaptured;
        
        _dispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();
        
        Title = "Ody-FakeHZ";
        
        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
        var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
        var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
        
        if (appWindow != null)
        {
            var presenter = appWindow.Presenter as Microsoft.UI.Windowing.OverlappedPresenter;
            if (presenter != null)
            {
                presenter.IsResizable = true;
                presenter.IsMaximizable = true;
                presenter.IsMinimizable = true;
            }
        }

        LoadActiveProfile();
        HighlightProfileCard(GamingProfileCard);
        LoadMonitorInfo();
    }

    private void LoadMonitorInfo()
    {
        try
        {
            var info = _displayManager.GetCurrentMonitorInfo();
            MonitorInfoText.Text = $"Display: {info}";
        }
        catch
        {
            MonitorInfoText.Text = "Display: Unable to detect";
        }
    }

    private void OnFrameCaptured(object? sender, CapturedFrame frame)
    {
        _performanceMonitor.RecordFrame();
    }

    private void OnMetricsUpdated(object? sender, PerformanceData data)
    {
        _dispatcherQueue?.TryEnqueue(() =>
        {
            GpuUsageText.Text = $"{data.GpuUsage:F0}%";
            LatencyText.Text = _isEnabled ? $"{data.LatencyMs:F1}ms" : "—";
            
            if (_isEnabled)
            {
                EngineStatusText.Text = "Active";
                EngineStatusText.Foreground = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["TextPrimaryBrush"];
            }
        });
    }

    private void LoadActiveProfile()
    {
        var profile = _profileManager.ActiveProfile;
        if (profile != null)
        {
            HzSliderControl.Value = profile.TargetHz;
            UpdateStatusForProfile(profile);
        }
    }

    private void OnProfileChanged(object? sender, Profile profile)
    {
        HzSliderControl.Value = profile.TargetHz;
        UpdateStatusForProfile(profile);
    }

    private void UpdateStatusForProfile(Profile profile)
    {
        if (_isEnabled)
        {
            StatusText.Text = $"Active • {profile.Name} • {profile.TargetHz}Hz";
        }
        else
        {
            StatusText.Text = $"{profile.Name} profile • Ready";
        }
    }

    private void EnableButton_Click(object sender, RoutedEventArgs e)
    {
        _isEnabled = !_isEnabled;

        if (_isEnabled)
        {
            try
            {
                EnableButton.Content = "DISABLE ODY-FAKEHZ";
                
                // Validate target Hz
                var profile = _profileManager.ActiveProfile;
                if (profile != null && !_displayManager.ValidateRefreshRate(profile.TargetHz))
                {
                    // Show warning but continue
                    StatusText.Text = $"Warning: {profile.TargetHz}Hz may exceed monitor capability";
                }

                // Start monitoring and capture
                _performanceMonitor.StartMonitoring();
                _captureManager.StartCapture(60);

                if (profile != null)
                {
                    StatusText.Text = $"Active • {profile.Name} • {profile.TargetHz}Hz";
                }
                
                EngineStatusText.Text = "Active";
                EngineStatusText.Foreground = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["TextPrimaryBrush"];
            }
            catch (Exception ex)
            {
                _isEnabled = false;
                EnableButton.Content = "ENABLE ODY-FAKEHZ";
                StatusText.Text = $"Error: {ex.Message}";
            }
        }
        else
        {
            EnableButton.Content = "ENABLE ODY-FAKEHZ";
            
            _performanceMonitor.StopMonitoring();
            _captureManager.StopCapture();
            
            var profile = _profileManager.ActiveProfile;
            if (profile != null)
            {
                StatusText.Text = $"{profile.Name} profile • Ready";
            }
            
            GpuUsageText.Text = "0%";
            LatencyText.Text = "—";
            EngineStatusText.Text = "Idle";
            EngineStatusText.Foreground = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["TextSecondaryBrush"];
        }
    }

    private void ModeToggle_Toggled(object sender, RoutedEventArgs e)
    {
        var toggle = sender as ToggleSwitch;
        if (toggle == null) return;

        AdvancedSettings.Visibility = toggle.IsOn ? Visibility.Visible : Visibility.Collapsed;
    }

    private void ProfileCard_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
        var card = sender as Border;
        if (card == null) return;

        var profileName = card.Tag as string ?? "Gaming";
        _profileManager.SetActiveProfile(profileName);

        ResetProfileCards();
        HighlightProfileCard(card);
    }

    private void HzSlider_ValueChanged(object? sender, int value)
    {
        _profileManager.UpdateActiveProfile(profile => profile.TargetHz = value);
        
        if (_isEnabled)
        {
            StatusText.Text = $"Active • {_profileManager.ActiveProfile?.Name} • {value}Hz";
        }
    }

    private void ResetProfileCards()
    {
        GamingProfileCard.Background = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["SurfaceBrush"];
        MoviesProfileCard.Background = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["SurfaceBrush"];
        DesktopProfileCard.Background = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["SurfaceBrush"];
    }

    private void HighlightProfileCard(Border card)
    {
        card.Background = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["SurfaceElevatedBrush"];
    }
}
