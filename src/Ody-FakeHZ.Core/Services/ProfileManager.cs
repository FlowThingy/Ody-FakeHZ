using System.Text.Json;
using OdyFakeHZ.Core.Models;

namespace OdyFakeHZ.Core.Services;

public class ProfileManager
{
    private readonly string _profilesPath;
    private List<Profile> _profiles;
    private Profile? _activeProfile;

    public ProfileManager()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var appFolder = Path.Combine(appData, "Ody-FakeHZ");
        Directory.CreateDirectory(appFolder);
        
        _profilesPath = Path.Combine(appFolder, "profiles.json");
        _profiles = new List<Profile>();
        
        LoadProfiles();
    }

    public IReadOnlyList<Profile> Profiles => _profiles.AsReadOnly();
    
    public Profile? ActiveProfile => _activeProfile;

    public event EventHandler<Profile>? ProfileChanged;

    public void LoadProfiles()
    {
        if (File.Exists(_profilesPath))
        {
            try
            {
                var json = File.ReadAllText(_profilesPath);
                var loaded = JsonSerializer.Deserialize<List<Profile>>(json);
                if (loaded != null)
                {
                    _profiles = loaded;
                }
            }
            catch
            {
                // If load fails, just use defaults
            }
        }

        // Always ensure default profiles exist
        EnsureDefaultProfiles();
        
        // Set Gaming as default active profile
        _activeProfile = _profiles.FirstOrDefault(p => p.Name == "Gaming");
    }

    public void SaveProfiles()
    {
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(_profiles, options);
            File.WriteAllText(_profilesPath, json);
        }
        catch
        {
            // Silently fail for now
        }
    }

    public void SetActiveProfile(string profileName)
    {
        var profile = _profiles.FirstOrDefault(p => p.Name == profileName);
        if (profile != null)
        {
            _activeProfile = profile;
            ProfileChanged?.Invoke(this, profile);
        }
    }

    public void UpdateActiveProfile(Action<Profile> updateAction)
    {
        if (_activeProfile != null)
        {
            updateAction(_activeProfile);
            SaveProfiles();
            ProfileChanged?.Invoke(this, _activeProfile);
        }
    }

    public void AddCustomProfile(Profile profile)
    {
        if (!_profiles.Any(p => p.Name == profile.Name))
        {
            _profiles.Add(profile);
            SaveProfiles();
        }
    }

    public void DeleteProfile(string profileName)
    {
        var profile = _profiles.FirstOrDefault(p => p.Name == profileName);
        if (profile != null && !profile.IsBuiltIn)
        {
            _profiles.Remove(profile);
            SaveProfiles();
        }
    }

    private void EnsureDefaultProfiles()
    {
        if (!_profiles.Any(p => p.Name == "Gaming"))
        {
            _profiles.Add(Profile.CreateGamingProfile());
        }
        
        if (!_profiles.Any(p => p.Name == "Movies"))
        {
            _profiles.Add(Profile.CreateMoviesProfile());
        }
        
        if (!_profiles.Any(p => p.Name == "Desktop"))
        {
            _profiles.Add(Profile.CreateDesktopProfile());
        }
    }
}
