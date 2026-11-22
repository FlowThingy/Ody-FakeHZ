using System.Runtime.InteropServices;

namespace OdyFakeHZ.Core.Services;

public class DisplayManager
{
    [StructLayout(LayoutKind.Sequential)]
    public struct DEVMODE
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string dmDeviceName;
        public short dmSpecVersion;
        public short dmDriverVersion;
        public short dmSize;
        public short dmDriverExtra;
        public int dmFields;
        public int dmPositionX;
        public int dmPositionY;
        public int dmDisplayOrientation;
        public int dmDisplayFixedOutput;
        public short dmColor;
        public short dmDuplex;
        public short dmYResolution;
        public short dmTTOption;
        public short dmCollate;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string dmFormName;
        public short dmLogPixels;
        public int dmBitsPerPel;
        public int dmPelsWidth;
        public int dmPelsHeight;
        public int dmDisplayFlags;
        public int dmDisplayFrequency;
        public int dmICMMethod;
        public int dmICMIntent;
        public int dmMediaType;
        public int dmDitherType;
        public int dmReserved1;
        public int dmReserved2;
        public int dmPanningWidth;
        public int dmPanningHeight;
    }

    [DllImport("user32.dll")]
    private static extern bool EnumDisplaySettings(string? deviceName, int modeNum, ref DEVMODE devMode);

    [DllImport("user32.dll")]
    private static extern int ChangeDisplaySettings(ref DEVMODE devMode, int flags);

    private const int ENUM_CURRENT_SETTINGS = -1;
    private const int ENUM_REGISTRY_SETTINGS = -2;

    public MonitorInfo GetCurrentMonitorInfo()
    {
        var devMode = new DEVMODE();
        devMode.dmSize = (short)Marshal.SizeOf(devMode);

        if (EnumDisplaySettings(null, ENUM_CURRENT_SETTINGS, ref devMode))
        {
            return new MonitorInfo
            {
                Width = devMode.dmPelsWidth,
                Height = devMode.dmPelsHeight,
                RefreshRate = devMode.dmDisplayFrequency,
                BitsPerPixel = devMode.dmBitsPerPel
            };
        }

        return new MonitorInfo();
    }

    public List<MonitorMode> GetAvailableModes()
    {
        var modes = new List<MonitorMode>();
        var devMode = new DEVMODE();
        devMode.dmSize = (short)Marshal.SizeOf(devMode);

        int modeNum = 0;
        while (EnumDisplaySettings(null, modeNum, ref devMode))
        {
            var mode = new MonitorMode
            {
                Width = devMode.dmPelsWidth,
                Height = devMode.dmPelsHeight,
                RefreshRate = devMode.dmDisplayFrequency,
                BitsPerPixel = devMode.dmBitsPerPel
            };

            if (!modes.Any(m => m.Width == mode.Width && 
                               m.Height == mode.Height && 
                               m.RefreshRate == mode.RefreshRate))
            {
                modes.Add(mode);
            }

            modeNum++;
        }

        return modes.OrderByDescending(m => m.RefreshRate)
                   .ThenByDescending(m => m.Width)
                   .ToList();
    }

    public bool ValidateRefreshRate(int targetHz)
    {
        var currentInfo = GetCurrentMonitorInfo();
        
        // Allow target Hz between current rate and 240Hz
        return targetHz >= currentInfo.RefreshRate && targetHz <= 240;
    }

    public int GetMaxSupportedRefreshRate()
    {
        var modes = GetAvailableModes();
        var currentInfo = GetCurrentMonitorInfo();
        
        return modes
            .Where(m => m.Width == currentInfo.Width && m.Height == currentInfo.Height)
            .Max(m => m.RefreshRate);
    }
}

public class MonitorInfo
{
    public int Width { get; set; }
    public int Height { get; set; }
    public int RefreshRate { get; set; }
    public int BitsPerPixel { get; set; }

    public override string ToString()
    {
        return $"{Width}x{Height} @ {RefreshRate}Hz ({BitsPerPixel}bit)";
    }
}

public class MonitorMode
{
    public int Width { get; set; }
    public int Height { get; set; }
    public int RefreshRate { get; set; }
    public int BitsPerPixel { get; set; }

    public override string ToString()
    {
        return $"{Width}x{Height} @ {RefreshRate}Hz";
    }
}
