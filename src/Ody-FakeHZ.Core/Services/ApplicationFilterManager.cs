using System.Diagnostics;
using System.Text.Json;

namespace OdyFakeHZ.Core.Services;

public class ApplicationFilterManager
{
    private readonly string _filePath;
    private HashSet<string> _enabledApps;
    private HashSet<string> _disabledApps;

    public IReadOnlyCollection<string> EnabledApps => _enabledApps;
    public IReadOnlyCollection<string> DisabledApps => _disabledApps;

    public event EventHandler? FiltersChanged;

    public ApplicationFilterManager()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var appFolder = Path.Combine(appData, "Ody-FakeHZ");
        Directory.CreateDirectory(appFolder);
        
        _filePath = Path.Combine(appFolder, "app-filters.json");
        _enabledApps = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        _disabledApps = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        
        Load();
    }

    public void Load()
    {
        if (File.Exists(_filePath))
        {
            try
            {
                var json = File.ReadAllText(_filePath);
                var data = JsonSerializer.Deserialize<FilterData>(json);
                
                if (data != null)
                {
                    _enabledApps = new HashSet<string>(data.EnabledApps, StringComparer.OrdinalIgnoreCase);
                    _disabledApps = new HashSet<string>(data.DisabledApps, StringComparer.OrdinalIgnoreCase);
                }
            }
            catch { }
        }
    }

    public void Save()
    {
        try
        {
            var data = new FilterData
            {
                EnabledApps = _enabledApps.ToList(),
                DisabledApps = _disabledApps.ToList()
            };

            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(data, options);
            File.WriteAllText(_filePath, json);
        }
        catch { }
    }

    public bool IsAppEnabled(string processName)
    {
        if (_disabledApps.Contains(processName))
            return false;

        // If no filters set, enable all
        if (_enabledApps.Count == 0 && _disabledApps.Count == 0)
            return true;

        return _enabledApps.Contains(processName);
    }

    public void EnableApp(string processName)
    {
        _disabledApps.Remove(processName);
        _enabledApps.Add(processName);
        Save();
        FiltersChanged?.Invoke(this, EventArgs.Empty);
    }

    public void DisableApp(string processName)
    {
        _enabledApps.Remove(processName);
        _disabledApps.Add(processName);
        Save();
        FiltersChanged?.Invoke(this, EventArgs.Empty);
    }

    public void ClearFilters()
    {
        _enabledApps.Clear();
        _disabledApps.Clear();
        Save();
        FiltersChanged?.Invoke(this, EventArgs.Empty);
    }

    public List<RunningApplication> GetRunningApplications()
    {
        var apps = new List<RunningApplication>();
        var processes = Process.GetProcesses();

        foreach (var process in processes)
        {
            try
            {
                if (string.IsNullOrEmpty(process.MainWindowTitle) || 
                    process.MainWindowHandle == IntPtr.Zero)
                    continue;

                apps.Add(new RunningApplication
                {
                    ProcessName = process.ProcessName,
                    WindowTitle = process.MainWindowTitle,
                    ProcessId = process.Id,
                    IsEnabled = IsAppEnabled(process.ProcessName)
                });
            }
            catch
            {
                // Skip processes we can't access
            }
        }

        return apps.OrderBy(a => a.ProcessName).ToList();
    }

    private class FilterData
    {
        public List<string> EnabledApps { get; set; } = new();
        public List<string> DisabledApps { get; set; } = new();
    }
}

public class RunningApplication
{
    public string ProcessName { get; set; } = string.Empty;
    public string WindowTitle { get; set; } = string.Empty;
    public int ProcessId { get; set; }
    public bool IsEnabled { get; set; }
}
