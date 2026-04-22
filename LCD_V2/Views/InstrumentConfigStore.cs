using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace LCD_V2.Views
{
    /// <summary>
    /// Per-instrument tuning persistence (single file for all instruments).
    /// %AppData%\LCD_V2\instruments.xml — one <InstrumentConfig> entry per instrument key.
    /// </summary>
    public static class InstrumentConfigStore
    {
        private static readonly string _path;
        private static readonly Dictionary<string, InstrumentConfig> _byKey
            = new Dictionary<string, InstrumentConfig>(StringComparer.OrdinalIgnoreCase);

        static InstrumentConfigStore()
        {
            var dir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "LCD_V2");
            try { Directory.CreateDirectory(dir); } catch { /* best effort */ }
            _path = Path.Combine(dir, "instruments.xml");

            Load();
            SeedMissing();
            Save();
        }

        /// <summary>Get the config for an instrument key; creates a default entry if missing.</summary>
        public static InstrumentConfig Get(string instrumentKey)
        {
            if (string.IsNullOrEmpty(instrumentKey)) instrumentKey = "BMA7";
            if (!_byKey.TryGetValue(instrumentKey, out var cfg))
            {
                cfg = new InstrumentConfig { Instrument = instrumentKey };
                _byKey[instrumentKey] = cfg;
            }
            return cfg;
        }

        /// <summary>Save an updated config (replaces the existing entry for its Instrument key).</summary>
        public static void Put(InstrumentConfig cfg)
        {
            if (cfg == null || string.IsNullOrEmpty(cfg.Instrument)) return;
            _byKey[cfg.Instrument] = cfg;
            Save();
        }

        private static void Load()
        {
            _byKey.Clear();
            if (!File.Exists(_path)) return;
            try
            {
                var ser = new XmlSerializer(typeof(InstrumentConfigBag));
                using (var fs = File.OpenRead(_path))
                {
                    if (ser.Deserialize(fs) is InstrumentConfigBag bag && bag.Items != null)
                        foreach (var c in bag.Items)
                            if (!string.IsNullOrEmpty(c.Instrument))
                                _byKey[c.Instrument] = c;
                }
            }
            catch
            {
                // ignore corrupted file — reseed below
            }
        }

        private static void SeedMissing()
        {
            foreach (var info in InstrumentCatalog.All)
            {
                if (!_byKey.ContainsKey(info.Key))
                    _byKey[info.Key] = new InstrumentConfig { Instrument = info.Key };
            }
        }

        private static void Save()
        {
            try
            {
                var bag = new InstrumentConfigBag { Items = _byKey.Values.OrderBy(c => c.Instrument).ToList() };
                var ser = new XmlSerializer(typeof(InstrumentConfigBag));
                var tmp = _path + ".tmp";
                using (var fs = File.Create(tmp))
                {
                    ser.Serialize(fs, bag);
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
