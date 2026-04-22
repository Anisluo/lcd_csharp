using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Xml.Serialization;
using LCD.Core.Services;

namespace LCD_V2.Views
{
    /// <summary>
    /// Shared template library. One static ObservableCollection survives page
    /// re-creation and is transparently persisted to XML at
    ///   %AppData%\LCD_V2\templates.xml
    /// — SQLite is overkill for a handful of templates; XML is readable and
    /// has no dependency. (Test-result data, which is high-volume, still uses
    /// SQLite on the LCD V1 side.)
    /// </summary>
    public static class TemplateStore
    {
        private static readonly string _path;
        private static bool _suspendAutoSave;

        public static ObservableCollection<TemplateItem> Library { get; }

        static TemplateStore()
        {
            var dir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "LCD_V2");
            try { Directory.CreateDirectory(dir); } catch { /* best effort */ }
            _path = Path.Combine(dir, "templates.xml");

            // Build the collection first, THEN expose it via Library.
            // If we seed while Library is still null, the re-entrant Save()
            // chokes on `new List<>(Library)` — ArgumentNullException.
            _suspendAutoSave = true;
            var col = Load();
            bool wasEmpty = col.Count == 0;
            if (wasEmpty) Seed(col);
            Library = col;
            _suspendAutoSave = false;

            Library.CollectionChanged += OnLibraryChanged;
            if (wasEmpty) Save(); // persist seed now that Library is live
        }

        private static ObservableCollection<TemplateItem> Load()
        {
            var col = new ObservableCollection<TemplateItem>();
            if (File.Exists(_path))
            {
                try
                {
                    var ser = new XmlSerializer(typeof(List<TemplateItem>));
                    using (var fs = File.OpenRead(_path))
                    {
                        if (ser.Deserialize(fs) is List<TemplateItem> items)
                            foreach (var it in items) col.Add(it);
                    }
                }
                catch
                {
                    // corrupted file — silently ignore, fall through to seed
                }
            }
            return col;
        }

        private static void Seed(ObservableCollection<TemplateItem> col)
        {
            col.Add(new TemplateItem { Name = "13 寸屏 · 对角", ConfigType = PointLayoutType.Point13Diag, H = 286, V = 179, A = 10, B = 10, C = 25, D = 25, PointCount = 13 });
            col.Add(new TemplateItem { Name = "中控屏 9 点",   ConfigType = PointLayoutType.Point9,      H = 250, V = 150, A = 10, B = 10, C = 25, D = 25, PointCount =  9 });
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
                var ser = new XmlSerializer(typeof(List<TemplateItem>));
                var tmp = _path + ".tmp";
                using (var fs = File.Create(tmp))
                {
                    ser.Serialize(fs, new List<TemplateItem>(Library));
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

        /// <summary>Force a write for the case where you mutated an existing item in place
        /// (rare — normally replace the item to trigger CollectionChanged).</summary>
        public static void Flush() => Save();
    }
}
