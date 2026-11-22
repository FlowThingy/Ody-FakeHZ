using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;

namespace OdyFakeHZ.UI.Controls;

public sealed partial class HzSlider : UserControl
{
    public event EventHandler<int>? ValueChanged;

    public HzSlider()
    {
        this.InitializeComponent();
    }

    public int Value
    {
        get => (int)MainSlider.Value;
        set
        {
            MainSlider.Value = value;
            UpdateDisplay(value);
        }
    }

    private void MainSlider_ValueChanged(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
    {
        var roundedValue = (int)Math.Round(e.NewValue);
        
        // Snap to common values for better UX
        roundedValue = SnapToCommonValue(roundedValue);
        
        UpdateDisplay(roundedValue);
        AnimateValueChange();
        
        ValueChanged?.Invoke(this, roundedValue);
    }

    private int SnapToCommonValue(int value)
    {
        // Snap to common refresh rates if close enough
        int[] commonValues = { 60, 75, 90, 120, 144, 165, 240 };
        
        foreach (var common in commonValues)
        {
            if (Math.Abs(value - common) <= 2)
            {
                return common;
            }
        }
        
        return value;
    }

    private void UpdateDisplay(int value)
    {
        HzValueText.Text = value.ToString();
    }

    private void AnimateValueChange()
    {
        // Small pulse animation when value changes
        var storyboard = new Storyboard();
        
        var scaleXAnim = new DoubleAnimation
        {
            From = 1.0,
            To = 1.1,
            Duration = TimeSpan.FromMilliseconds(100),
            AutoReverse = true
        };
        
        var scaleYAnim = new DoubleAnimation
        {
            From = 1.0,
            To = 1.1,
            Duration = TimeSpan.FromMilliseconds(100),
            AutoReverse = true
        };
        
        Storyboard.SetTarget(scaleXAnim, HzTextScale);
        Storyboard.SetTargetProperty(scaleXAnim, "ScaleX");
        
        Storyboard.SetTarget(scaleYAnim, HzTextScale);
        Storyboard.SetTargetProperty(scaleYAnim, "ScaleY");
        
        storyboard.Children.Add(scaleXAnim);
        storyboard.Children.Add(scaleYAnim);
        storyboard.Begin();
    }
}
