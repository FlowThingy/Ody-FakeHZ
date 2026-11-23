using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using OdyFakeHZ.Core.Services;
using OdyFakeHZ.Core.Models;
using OdyFakeHZ.Engine.Capture;
using OdyFakeHZ.Engine;
using OdyFakeHZ.Engine.Compositor;
using OdyFakeHZ.UI.Services;
using System;
using CoreProcessingMode = OdyFakeHZ.Core.Models.ProcessingMode;
using EngineProcessingMode = OdyFakeHZ.Engine.ProcessingMode;

namespace OdyFakeHZ.UI;

public sealed partial class MainWindow : Window
{
    private bool _isEnabled = false;
    private readonly ProfileManager _profileManager;
    private readonly PerformanceMonitor _performanceMonitor;
    private readonly DisplayManager _displayManager;
    private readonly DirectXCaptureManager _captureManager;
    private readonly InterpolationEngine _interpolationEngine;
    private readonly CompositorManager _compositorManager;
    private readonly ApplicationFilterManager _appFilterManager;
    private readonly SettingsManager _settingsManager;
    private readonly TrayIconManager _trayIcon;
    private Microsoft.UI.Dispatching.DispatcherQueue? _dispatcherQueue;

    public MainWindow()
    {
        this.InitializeComponent();
        
        // Set window size (WinUI 3 doesn't support Width/Height in XAML)
        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
        var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
        var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
        
        if (appWindow != null)
        {
            appWindow.Resize(new Windows.Graphics.SizeInt32 { Width = 1200, Height = 800 });
            
            var presenter = appWindow.Presenter as Microsoft.UI.Windowing.OverlappedPresenter;
            if (presenter != null)
            {
                presenter.IsResizable = true;
                presenter.IsMaximizable = true;
                presenter.IsMinimizable = true;
            }
        }
        
        Title = "Ody-FakeHZ";
        _profileManager.ProfileChanged += OnProfileChanged;
        
        _performanceMonitor = new PerformanceMonitor();
        _performanceMonitor.MetricsUpdated += OnMetricsUpdated;

        _displayManager = new DisplayManager();
        
        _captureManager = new DirectXCaptureManager();
        _captureManager.FrameCaptured += OnFrameCaptured;
        
        _interpolationEngine = new InterpolationEngine();
        _interpolationEngine.FrameGenerated += OnFrameGenerated;
        
        _compositorManager = new CompositorManager();
        _compositorManager.StatusChanged += OnCompositorStatus;

        _appFilterManager = new ApplicationFilterManager();
        _settingsManager = new SettingsManager();

        _trayIcon = new TrayIconManager(this);
        _trayIcon.EnableClicked += (s, e) => EnableOdyFakeHZ();
        _trayIcon.DisableClicked += (s, e) => DisableOdyFakeHZ();
        _trayIcon.ShowWindowClicked += (s, e) => ShowMainWindow();
        _trayIcon.ExitClicked += (s, e) => Application.Current.Exit();
        _trayIcon.ProfileClicked += (s, profileName) => SwitchProfile(profileName);
        _trayIcon.Initialize();
        
        _dispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();
        
        Title = "Ody-FakeHZ";
        
        _profileManager = new ProfileManager();

        
        this.Closed += OnWindowClosed;

        LoadSettings();
        LoadActiveProfile();
        HighlightProfileCard(GamingProfileCard);
        LoadMonitorInfo();
    }

    private void LoadSettings()
    {
        // Settings loading will be implemented in Task 10
    }

    private void OnWindowClosed(object sender, WindowEventArgs args)
    {
        _trayIcon.Dispose();
        _compositorManager.Stop();
        _interpolationEngine.Stop();
        _performanceMonitor.StopMonitoring();
        _captureManager.StopCapture();
    }

    private void ShowMainWindow()
    {
        this.Activate();
    }

    private void SwitchProfile(string profileName)
    {
        _profileManager.SetActiveProfile(profileName);
        _trayIcon.ShowNotification("Profile Changed", $"Switched to {profileName} profile");
    }

    private void OnCompositorStatus(object? sender, string status)
    {
        // Log compositor status for debugging
        System.Diagnostics.Debug.WriteLine($"Compositor: {status}");
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
        
        if (_interpolationEngine.IsRunning)
        {
            _interpolationEngine.ProcessCapturedFrame(frame);
        }
    }

    private void OnFrameGenerated(object? sender, OdyFakeHZ.Engine.Interpolation.InterpolatedFrame frame)
    {
        if (_compositorManager.IsRunning)
        {
            _compositorManager.PresentFrame(frame);
        }
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
            QualitySlider.Value = profile.QualityLevel;
            
            // Set processing mode radio buttons
            switch (profile.ProcessingMode)
            {
                case CoreProcessingMode.GPU:
                    GpuModeRadio.IsChecked = true;
                    break;
                case CoreProcessingMode.Hybrid:
                    HybridModeRadio.IsChecked = true;
                    break;
                case CoreProcessingMode.CPU:
                    CpuModeRadio.IsChecked = true;
                    break;
            }
            
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
        if (!_isEnabled)
        {
            EnableOdyFakeHZ();
        }
        else
        {
            DisableOdyFakeHZ();
        }
    }

    private void EnableOdyFakeHZ()
    {
        _isEnabled = true;

        try
        {
            var profile = _profileManager.ActiveProfile;
            if (profile == null) return;

            EnableButton.Content = "DISABLE ODY-FAKEHZ";
            
            if (!_displayManager.ValidateRefreshRate(profile.TargetHz))
            {
                StatusText.Text = $"Warning: {profile.TargetHz}Hz may exceed monitor capability";
            }

            var currentHz = _displayManager.GetCurrentMonitorInfo().RefreshRate;
            var multiplier = Math.Max(1, profile.TargetHz / currentHz);

            if (!_compositorManager.IsInitialized)
            {
                if (!_compositorManager.Initialize())
                {
                    throw new Exception("Failed to initialize compositor");
                }
            }

            var mode = profile.ProcessingMode switch
            {
                CoreProcessingMode.GPU => EngineProcessingMode.GPU,
                CoreProcessingMode.Hybrid => EngineProcessingMode.Hybrid,
                _ => EngineProcessingMode.CPU
            };

            if (_interpolationEngine.Initialize(mode))
            {
                _interpolationEngine.TargetMultiplier = multiplier;
                _interpolationEngine.Quality = profile.QualityLevel switch
                {
                    >= 80 => OdyFakeHZ.Engine.Interpolation.InterpolationQuality.High,
                    >= 40 => OdyFakeHZ.Engine.Interpolation.InterpolationQuality.Balanced,
                    _ => OdyFakeHZ.Engine.Interpolation.InterpolationQuality.Draft
                };
                
                _interpolationEngine.Start();
            }

            _compositorManager.Start();
            _performanceMonitor.StartMonitoring();
            _captureManager.StartCapture(currentHz);

            StatusText.Text = $"Active • {profile.Name} • {profile.TargetHz}Hz";
            EngineStatusText.Text = "Active";
            EngineStatusText.Foreground = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["TextPrimaryBrush"];
            
            _trayIcon.SetStatus(true);
            _trayIcon.ShowNotification("Ody-FakeHZ", $"Enabled with {profile.Name} profile");
        }
        catch (Exception ex)
        {
            _isEnabled = false;
            EnableButton.Content = "ENABLE ODY-FAKEHZ";
            StatusText.Text = $"Error: {ex.Message}";
            _trayIcon.SetStatus(false);
        }
    }

    private void DisableOdyFakeHZ()
    {
        _isEnabled = false;
        EnableButton.Content = "ENABLE ODY-FAKEHZ";
        
        _compositorManager.Stop();
        _interpolationEngine.Stop();
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
        
        _trayIcon.SetStatus(false);
    }

    private void ModeToggle_Toggled(object sender, RoutedEventArgs e)
    {
        var toggle = sender as ToggleSwitch;
        if (toggle == null) return;

        if (toggle.IsOn)
        {
            AdvancedSettings.Visibility = Visibility.Visible;
            LoadRunningApps();
        }
        else
        {
            AdvancedSettings.Visibility = Visibility.Collapsed;
        }
    }

    private void ProcessingMode_Changed(object sender, RoutedEventArgs e)
    {
        var profile = _profileManager.ActiveProfile;
        if (profile == null) return;

        if (GpuModeRadio.IsChecked == true)
        {
            profile.ProcessingMode = CoreProcessingMode.GPU;
        }
        else if (HybridModeRadio.IsChecked == true)
        {
            profile.ProcessingMode = CoreProcessingMode.Hybrid;
        }
        else if (CpuModeRadio.IsChecked == true)
        {
            profile.ProcessingMode = CoreProcessingMode.CPU;
        }

        _profileManager.UpdateActiveProfile(p => p.ProcessingMode = profile.ProcessingMode);
    }

    private void QualitySlider_ValueChanged(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
    {
        var value = (int)e.NewValue;
        QualityValueText.Text = $"Quality: {value}";
        
        _profileManager.UpdateActiveProfile(p => p.QualityLevel = value);
    }

    private void RefreshApps_Click(object sender, RoutedEventArgs e)
    {
        LoadRunningApps();
    }

    private void LoadRunningApps()
    {
        AppListPanel.Children.Clear();
        
        var apps = _appFilterManager.GetRunningApplications();
        
        if (apps.Count == 0)
        {
            var noApps = new TextBlock
            {
                Text = "No running applications with windows found",
                Style = (Style)Application.Current.Resources["CaptionTextStyle"],
                Margin = new Thickness(0, 8, 0, 8)
            };
            AppListPanel.Children.Add(noApps);
            return;
        }

        foreach (var app in apps)
        {
            var appCard = new Border
            {
                Style = (Style)Application.Current.Resources["CardStyle"],
                Padding = new Thickness(12),
                Margin = new Thickness(0, 0, 0, 4)
            };

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            var textStack = new StackPanel { Spacing = 4 };
            
            var nameText = new TextBlock
            {
                Text = app.ProcessName,
                Style = (Style)Application.Current.Resources["BodyTextStyle"],
                Foreground = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["TextPrimaryBrush"]
            };
            
            var titleText = new TextBlock
            {
                Text = app.WindowTitle,
                Style = (Style)Application.Current.Resources["CaptionTextStyle"],
                TextTrimming = TextTrimming.CharacterEllipsis,
                MaxLines = 1
            };

            textStack.Children.Add(nameText);
            textStack.Children.Add(titleText);

            var toggle = new ToggleSwitch
            {
                IsOn = app.IsEnabled,
                OnContent = "",
                OffContent = ""
            };
            
            toggle.Toggled += (s, args) =>
            {
                if (toggle.IsOn)
                {
                    _appFilterManager.EnableApp(app.ProcessName);
                }
                else
                {
                    _appFilterManager.DisableApp(app.ProcessName);
                }
            };

            Grid.SetColumn(textStack, 0);
            Grid.SetColumn(toggle, 1);

            grid.Children.Add(textStack);
            grid.Children.Add(toggle);

            appCard.Child = grid;
            AppListPanel.Children.Add(appCard);
        }
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
