using System.Text.Json;

namespace OdyFakeHZ.Core.Services;

public class SettingsManager
{
    private readonly string _settingsPath;
    private AppSettings _settings;

    public AppSettings Settings => _settings;

    public event EventHandler? SettingsChanged;

    public SettingsManager()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var appFolder = Path.Combine(appData, "Ody-FakeHZ");
        Directory.CreateDirectory(appFolder);
        
        _settingsPath = Path.Combine(appFolder, "settings.json");
        _settings = new AppSettings();
        
        Load();
    }

    public void Load()
    {
        if (File.Exists(_settingsPath))
        {
            try
            {
                var json = File.ReadAllText(_settingsPath);
                var loaded = JsonSerializer.Deserialize<AppSettings>(json);
                if (loaded != null)
                {
                    _settings = loaded;
                }
            }
            catch
            {
                // Use defaults on error
            }
        }
    }

    public void Save()
    {
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(_settings, options);
            File.WriteAllText(_settingsPath, json);
            
            SettingsChanged?.Invoke(this, EventArgs.Empty);
        }
        catch
        {
            // Silent fail
        }
    }

    public void Set(Action<AppSettings> configure)
    {
        configure(_settings);
        Save();
    }
}

public class AppSettings
{
    public bool StartWithWindows { get; set; } = false;
    public bool StartMinimized { get; set; } = false;
    public bool MinimizeToTray { get; set; } = true;
    public bool EnableOnStartup { get; set; } = false;
    public string LastProfileName { get; set; } = "Gaming";
    public WindowPosition? LastWindowPosition { get; set; }
}

public class WindowPosition
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
}
