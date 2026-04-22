using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Xml.Serialization;

namespace LCD_V2.Views
{
    /// <summary>
    /// Motion-profile library — persisted singleton, XML-backed.
    /// Same pattern as TemplateStore / MetricStore.
    /// %AppData%\LCD_V2\motion.xml
    /// </summary>
    public static class MotionStore
    {
        private static readonly string _path;
        private static bool _suspendAutoSave;

        public static ObservableCollection<MotionProfile> Library { get; }

        static MotionStore()
        {
            var dir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "LCD_V2");
            try { Directory.CreateDirectory(dir); } catch { /* best effort */ }
            _path = Path.Combine(dir, "motion.xml");

            _suspendAutoSave = true;
            var col = Load();
            bool wasEmpty = col.Count == 0;
            if (wasEmpty) Seed(col);
            Library = col;
            _suspendAutoSave = false;

            Library.CollectionChanged += OnLibraryChanged;
            if (wasEmpty) Save();
        }

        private static ObservableCollection<MotionProfile> Load()
        {
            var col = new ObservableCollection<MotionProfile>();
            if (File.Exists(_path))
            {
                try
                {
                    var ser = new XmlSerializer(typeof(List<MotionProfile>));
                    using (var fs = File.OpenRead(_path))
                    {
                        if (ser.Deserialize(fs) is List<MotionProfile> items)
                            foreach (var it in items) col.Add(it);
                    }
                    BackfillAxisCodes(col);
                }
                catch
                {
                    // fall through to seed
                }
            }
            return col;
        }

        /// <summary>
        /// Legacy profiles (saved before AxisCode existed) deserialise with empty codes.
        /// Fill in pure-numeric 1..5 based on list position. Also strips the legacy "轴"
        /// prefix from earlier migrations so codes stay consistent with the new convention.
        /// InterpolateCode is left empty; user fills it in if a gantry dual-drive is needed.
        /// </summary>
        private static void BackfillAxisCodes(ObservableCollection<MotionProfile> col)
        {
            foreach (var p in col)
            {
                if (p.Axes == null) continue;
                for (int i = 0; i < p.Axes.Count; i++)
                {
                    var a = p.Axes[i];
                    if (string.IsNullOrWhiteSpace(a.AxisCode))
                        a.AxisCode = (i + 1).ToString();
                    else if (a.AxisCode.StartsWith("轴"))
                        a.AxisCode = a.AxisCode.Substring(1).Trim();

                    if (!string.IsNullOrWhiteSpace(a.InterpolateCode) && a.InterpolateCode.StartsWith("轴"))
                        a.InterpolateCode = a.InterpolateCode.Substring(1).Trim();
                }
            }
        }

        private static void Seed(ObservableCollection<MotionProfile> col)
        {
            col.Add(new MotionProfile
            {
                Name      = "标准 5 轴 (直动)",
                Algorithm = "Type D (直动 · 无补偿)",
                Axes      = MotionCatalog.NewDefaultAxes(),
            });

            col.Add(new MotionProfile
            {
                Name      = "中表旋转 (Type A)",
                Algorithm = "Type A (中表 · 旋转后补偿)",
                Axes      = MotionCatalog.NewDefaultAxes(),
            });
        }

        private static void OnLibraryChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_suspendAutoSave) return;
            Save();
        }

        private static void Save()
        {
            try
            {
                var ser = new XmlSerializer(typeof(List<MotionProfile>));
                var tmp = _path + ".tmp";
                using (var fs = File.Create(tmp))
                {
                    ser.Serialize(fs, new List<MotionProfile>(Library));
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

        public static void Flush() => Save();
    }
}
