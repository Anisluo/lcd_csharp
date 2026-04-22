using System;
using System.IO;
using System.Xml.Serialization;

namespace LCD_V2.Views
{
    /// <summary>
    /// Persisted dashboard state — remembers last-used template / metric / motion
    /// selections plus the crosshair offsets and jog step, so re-opening the app
    /// restores what the operator was looking at.
    ///
    /// Persisted at %AppData%\LCD_V2\dashboard.xml. Stored by NAME (not by index)
    /// so reordering the library doesn't silently switch the selection.
    /// </summary>
    public sealed class DashboardSettings
    {
        public string SelectedTemplateName { get; set; }
        public string SelectedMetricName   { get; set; }
        public string SelectedMotionName   { get; set; }

        // crosshair reference-line offsets — percent of viewport
        public double CrosshairHPct { get; set; } = 50.0;
        public double CrosshairVPct { get; set; } = 50.0;

        // jog step entered in the crosshair panel (percent)
        public string StepPercent { get; set; } = "1.0";
    }

    /// <summary>
    /// Persisted-singleton for DashboardSettings. Same file-backed pattern as
    /// TemplateStore / MetricStore / MotionStore — writes happen inline via Save().
    /// </summary>
    public static class DashboardSettingsStore
    {
        private static readonly string _path;
        public static DashboardSettings Current { get; }

        static DashboardSettingsStore()
        {
            var dir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "LCD_V2");
            try { Directory.CreateDirectory(dir); } catch { /* best effort */ }
            _path = Path.Combine(dir, "dashboard.xml");

            Current = Load() ?? new DashboardSettings();
        }

        private static DashboardSettings Load()
        {
            if (!File.Exists(_path)) return null;
            try
            {
                var ser = new XmlSerializer(typeof(DashboardSettings));
                using (var fs = File.OpenRead(_path))
                {
                    return ser.Deserialize(fs) as DashboardSettings;
                }
            }
            catch
            {
                return null; // corrupt file → fall back to defaults
            }
        }

        public static void Save()
        {
            try
            {
                var ser = new XmlSerializer(typeof(DashboardSettings));
                var tmp = _path + ".tmp";
                using (var fs = File.Create(tmp))
                {
                    ser.Serialize(fs, Current);
                }
                if (File.Exists(_path)) File.Delete(_path);
                File.Move(tmp, _path);
            }
            catch (Exception ex)
            {
                try
                {
                    File.WriteAllText(_path + ".error.log",
                        DateTime.Now + " - " + ex + Environment.NewLine);
                }
                catch { /* best effort */ }
            }
        }
    }
}
