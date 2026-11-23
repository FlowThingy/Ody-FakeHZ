using Microsoft.UI.Xaml;
using System.Drawing;

namespace OdyFakeHZ.UI.Services;

public class TrayIconManager : IDisposable
{
    private System.Windows.Forms.NotifyIcon? _notifyIcon;
    private readonly Window _mainWindow;
    private bool _isEnabled;

    public event EventHandler? EnableClicked;
    public event EventHandler? DisableClicked;
    public event EventHandler? ShowWindowClicked;
    public event EventHandler? ExitClicked;
    public event EventHandler<string>? ProfileClicked;

    public TrayIconManager(Window mainWindow)
    {
        _mainWindow = mainWindow;
    }

    public void Initialize()
    {
        _notifyIcon = new System.Windows.Forms.NotifyIcon
        {
            Text = "Ody-FakeHZ - Inactive",
            Visible = true
        };

        // Create a simple icon (we'd normally load from resources)
        _notifyIcon.Icon = CreateIcon();

        _notifyIcon.MouseClick += (s, e) =>
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                ShowWindow();
            }
        };

        UpdateContextMenu();
    }

    private Icon CreateIcon()
    {
        // Create a simple monochrome icon
        var bitmap = new Bitmap(16, 16);
        using (var g = Graphics.FromImage(bitmap))
        {
            g.Clear(Color.Black);
            g.FillEllipse(Brushes.White, 2, 2, 12, 12);
        }
        return Icon.FromHandle(bitmap.GetHicon());
    }

    private void UpdateContextMenu()
    {
        var menu = new System.Windows.Forms.ContextMenuStrip();

        // Status item
        var statusItem = new System.Windows.Forms.ToolStripMenuItem
        {
            Text = _isEnabled ? "Status: Active" : "Status: Inactive",
            Enabled = false
        };
        menu.Items.Add(statusItem);
        menu.Items.Add(new System.Windows.Forms.ToolStripSeparator());

        // Enable/Disable toggle
        var toggleItem = new System.Windows.Forms.ToolStripMenuItem
        {
            Text = _isEnabled ? "Disable" : "Enable"
        };
        toggleItem.Click += (s, e) =>
        {
            if (_isEnabled)
            {
                DisableClicked?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                EnableClicked?.Invoke(this, EventArgs.Empty);
            }
        };
        menu.Items.Add(toggleItem);
        menu.Items.Add(new System.Windows.Forms.ToolStripSeparator());

        // Profile submenu
        var profileMenu = new System.Windows.Forms.ToolStripMenuItem("Profiles");
        
        var gamingItem = new System.Windows.Forms.ToolStripMenuItem("Gaming");
        gamingItem.Click += (s, e) => ProfileClicked?.Invoke(this, "Gaming");
        profileMenu.DropDownItems.Add(gamingItem);

        var moviesItem = new System.Windows.Forms.ToolStripMenuItem("Movies");
        moviesItem.Click += (s, e) => ProfileClicked?.Invoke(this, "Movies");
        profileMenu.DropDownItems.Add(moviesItem);

        var desktopItem = new System.Windows.Forms.ToolStripMenuItem("Desktop");
        desktopItem.Click += (s, e) => ProfileClicked?.Invoke(this, "Desktop");
        profileMenu.DropDownItems.Add(desktopItem);

        menu.Items.Add(profileMenu);
        menu.Items.Add(new System.Windows.Forms.ToolStripSeparator());

        // Show window
        var showItem = new System.Windows.Forms.ToolStripMenuItem("Show Window");
        showItem.Click += (s, e) => ShowWindowClicked?.Invoke(this, EventArgs.Empty);
        menu.Items.Add(showItem);

        menu.Items.Add(new System.Windows.Forms.ToolStripSeparator());

        // Exit
        var exitItem = new System.Windows.Forms.ToolStripMenuItem("Exit");
        exitItem.Click += (s, e) => ExitClicked?.Invoke(this, EventArgs.Empty);
        menu.Items.Add(exitItem);

        if (_notifyIcon != null)
        {
            _notifyIcon.ContextMenuStrip = menu;
        }
    }

    public void SetStatus(bool enabled)
    {
        _isEnabled = enabled;
        
        if (_notifyIcon != null)
        {
            _notifyIcon.Text = enabled ? "Ody-FakeHZ - Active" : "Ody-FakeHZ - Inactive";
        }
        
        UpdateContextMenu();
    }

    public void ShowNotification(string title, string message)
    {
        _notifyIcon?.ShowBalloonTip(3000, title, message, System.Windows.Forms.ToolTipIcon.Info);
    }

    private void ShowWindow()
    {
        ShowWindowClicked?.Invoke(this, EventArgs.Empty);
    }

    public void Dispose()
    {
        if (_notifyIcon != null)
        {
            _notifyIcon.Visible = false;
            _notifyIcon.Dispose();
            _notifyIcon = null;
        }
    }
}
