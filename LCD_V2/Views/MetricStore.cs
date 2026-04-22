using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Xml.Serialization;

namespace LCD_V2.Views
{
    /// <summary>
    /// Instrument-metric library — persisted singleton, XML-backed.
    /// Same pattern as TemplateStore; file at %AppData%\LCD_V2\metrics.xml.
    /// </summary>
    public static class MetricStore
    {
        private static readonly string _path;
        private static bool _suspendAutoSave;

        public static ObservableCollection<InstrumentMetric> Library { get; }

        static MetricStore()
        {
            var dir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "LCD_V2");
            try { Directory.CreateDirectory(dir); } catch { /* best effort */ }
            _path = Path.Combine(dir, "metrics.xml");

            _suspendAutoSave = true;
            var col = Load();
            bool wasEmpty = col.Count == 0;
            if (wasEmpty) Seed(col);
            Library = col;
            _suspendAutoSave = false;

            Library.CollectionChanged += OnLibraryChanged;
            if (wasEmpty) Save();
        }

        private static ObservableCollection<InstrumentMetric> Load()
        {
            var col = new ObservableCollection<InstrumentMetric>();
            if (File.Exists(_path))
            {
                try
                {
                    var ser = new XmlSerializer(typeof(List<InstrumentMetric>));
                    using (var fs = File.OpenRead(_path))
                    {
                        if (ser.Deserialize(fs) is List<InstrumentMetric> items)
                            foreach (var it in items) col.Add(it);
                    }
                }
                catch
                {
                    // corrupt file → fall through to seed
                }
            }
            return col;
        }

        private static void Seed(ObservableCollection<InstrumentMetric> col)
        {
            col.Add(new InstrumentMetric
            {
                Name = "CS2000 标准 5 参",
                Instrument = "CS2000",
                Params = new List<MetricParam>
                {
                    new MetricParam { ParamName = "L (亮度)",     Enabled = true,  Algorithm = "均值" },
                    new MetricParam { ParamName = "Cx (色坐标)",  Enabled = true,  Algorithm = "均值" },
                    new MetricParam { ParamName = "Cy (色坐标)",  Enabled = true,  Algorithm = "均值" },
                    new MetricParam { ParamName = "CCT (色温)",   Enabled = true,  Algorithm = "均值" },
                    new MetricParam { ParamName = "Duv",           Enabled = true,  Algorithm = "均值" },
                },
            });

            col.Add(new InstrumentMetric
            {
                Name = "BM7A 快速 3 参",
                Instrument = "BMA7",
                Params = new List<MetricParam>
                {
                    new MetricParam { ParamName = "L (亮度)",    Enabled = true, Algorithm = "最大值" },
                    new MetricParam { ParamName = "Cx (色坐标)", Enabled = true, Algorithm = "均值"   },
                    new MetricParam { ParamName = "Cy (色坐标)", Enabled = true, Algorithm = "均值"   },
                },
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
                var ser = new XmlSerializer(typeof(List<InstrumentMetric>));
                var tmp = _path + ".tmp";
                using (var fs = File.Create(tmp))
                {
                    ser.Serialize(fs, new List<InstrumentMetric>(Library));
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
