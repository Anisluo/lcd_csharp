using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;

[assembly: AssemblyTitle("Microsoft.DwayneNeed")]
[assembly: AssemblyProduct("Microsoft.DwayneNeed")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: ComVisible(false)]

namespace Microsoft.DwayneNeed.Interop
{
    public enum AirspaceMode
    {
        None = 0,
        Redirect = 1,
    }

    /// <summary>
    /// Minimal shim — passes child through. Does not perform the actual airspace
    /// redirection from the original Microsoft.DwayneNeed sample.
    /// </summary>
    public class AirspaceDecorator : Decorator
    {
        public static readonly DependencyProperty AirspaceModeProperty =
            DependencyProperty.Register(nameof(AirspaceMode), typeof(AirspaceMode),
                typeof(AirspaceDecorator), new PropertyMetadata(AirspaceMode.None));

        public static readonly DependencyProperty IsInputRedirectionEnabledProperty =
            DependencyProperty.Register(nameof(IsInputRedirectionEnabled), typeof(bool),
                typeof(AirspaceDecorator), new PropertyMetadata(false));

        public static readonly DependencyProperty IsOutputRedirectionEnabledProperty =
            DependencyProperty.Register(nameof(IsOutputRedirectionEnabled), typeof(bool),
                typeof(AirspaceDecorator), new PropertyMetadata(false));

        public AirspaceMode AirspaceMode
        {
            get { return (AirspaceMode)GetValue(AirspaceModeProperty); }
            set { SetValue(AirspaceModeProperty, value); }
        }

        public bool IsInputRedirectionEnabled
        {
            get { return (bool)GetValue(IsInputRedirectionEnabledProperty); }
            set { SetValue(IsInputRedirectionEnabledProperty, value); }
        }

        public bool IsOutputRedirectionEnabled
        {
            get { return (bool)GetValue(IsOutputRedirectionEnabledProperty); }
            set { SetValue(IsOutputRedirectionEnabledProperty, value); }
        }
    }
}
