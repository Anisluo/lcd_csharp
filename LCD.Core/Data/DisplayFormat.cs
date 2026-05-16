using System;
using System.Globalization;

namespace LCD.Data
{
    /// <summary>
    /// Display formatter for measurement values that may be unmeasurable.
    /// Renders <see cref="double.NaN"/> (and the string "NaN" round-tripped through
    /// SQL persistence) as "*" — matches the BM-7A convention of returning
    /// asterisks for Tc / Duv when the colour falls outside the CCT locus
    /// (e.g. pure-blue test patterns).
    /// </summary>
    public static class DisplayFormat
    {
        public const string NaToken = "*";

        /// <summary>Format a numeric reading: "*" when NaN, otherwise the default ToString.</summary>
        public static string From(double v)
            => double.IsNaN(v) ? NaToken : v.ToString(CultureInfo.InvariantCulture);

        /// <summary>Format a numeric reading with a format spec; "*" when NaN.</summary>
        public static string From(double v, string fmt)
            => double.IsNaN(v) ? NaToken : v.ToString(fmt, CultureInfo.InvariantCulture);

        /// <summary>
        /// Boxed value suitable for a <c>DataRow</c> cell: "*" string when NaN,
        /// otherwise the original double. The DataTable typically has object/string
        /// columns so either type survives, and downstream string formatting picks
        /// up the "*" verbatim.
        /// </summary>
        public static object Cell(double v)
            => double.IsNaN(v) ? (object)NaToken : (object)v;

        /// <summary>
        /// Display-side normalisation for already-stringified values read from SQL
        /// (TestDataMode etc.). Returns "*" for "NaN" / "nan" / blank-asterisk
        /// payloads; otherwise the input unchanged.
        /// </summary>
        public static string FromString(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return s;
            var t = s.Trim();
            if (string.Equals(t, "NaN", StringComparison.OrdinalIgnoreCase)) return NaToken;
            if (t.Length > 0 && t[0] == '*') return NaToken;
            return s;
        }

        /// <summary>
        /// For Excel / DataRow cells loaded from string-typed SQL columns. Returns
        /// "*" for NaN-like inputs, a parsed double for valid numbers, or
        /// <paramref name="fallback"/> when the input is empty / null.
        /// </summary>
        public static object CellFromString(string s, double fallback = 0.0)
        {
            if (string.IsNullOrWhiteSpace(s)) return fallback;
            var t = s.Trim();
            if (string.Equals(t, "NaN", StringComparison.OrdinalIgnoreCase)) return NaToken;
            if (t.Length > 0 && t[0] == '*') return NaToken;
            double v;
            if (double.TryParse(t, NumberStyles.Float, CultureInfo.InvariantCulture, out v)) return v;
            return fallback;
        }
    }
}
