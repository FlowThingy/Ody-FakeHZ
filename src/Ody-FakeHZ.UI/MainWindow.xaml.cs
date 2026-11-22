using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace OdyFakeHZ.UI;

/// <summary>
/// The main application window.
/// </summary>
public sealed partial class MainWindow : Window
{
    private bool _isEnabled = false;
    private string _selectedProfile = "Gaming";

    public MainWindow()
    {
        this.InitializeComponent();
        
        // Set window properties
        Title = "Ody-FakeHZ";
        
        // Center the window on screen
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

        // Set initial profile selection
        HighlightProfileCard(GamingProfileCard);
    }

    private void EnableButton_Click(object sender, RoutedEventArgs e)
    {
        _isEnabled = !_isEnabled;

        if (_isEnabled)
        {
            EnableButton.Content = "DISABLE ODY-FAKEHZ";
            StatusText.Text = "Active • 120Hz Target • Processing";
        }
        else
        {
            EnableButton.Content = "ENABLE ODY-FAKEHZ";
            StatusText.Text = "Inactive • Ready to enable";
        }
    }

    private void ModeToggle_Toggled(object sender, RoutedEventArgs e)
    {
        var toggle = sender as ToggleSwitch;
        if (toggle == null) return;

        // Show/hide advanced settings based on toggle
        if (toggle.IsOn)
        {
            // Advanced mode
            AdvancedSettings.Visibility = Visibility.Visible;
        }
        else
        {
            // Simple mode
            AdvancedSettings.Visibility = Visibility.Collapsed;
        }
    }

    private void ProfileCard_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
        var card = sender as Border;
        if (card == null) return;

        _selectedProfile = card.Tag as string ?? "Gaming";

        // Update visual selection
        ResetProfileCards();
        HighlightProfileCard(card);

        // Load profile settings (placeholder for now)
        UpdateProfileSettings(_selectedProfile);
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

    private void UpdateProfileSettings(string profileName)
    {
        // Placeholder: In Task 3, this will load actual profile settings
        StatusText.Text = $"{profileName} profile selected • Ready to enable";
    }
}
