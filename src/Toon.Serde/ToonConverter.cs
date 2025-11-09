using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Toon.Serde
{
    // Canonical TOON serializer: JSON (JToken) -> TOON text
    public static class ToonConverter
    {
        public static string ToToon(JToken token, ToonOptions? opts = null)
        {
            opts = opts ?? new ToonOptions();
            var sb = new StringBuilder();

            if (token.Type == JTokenType.Object)
            {
                // top-level object special-case
                var obj = (JObject)token;
                var props = obj.Properties().ToList();
                if (props.Count == 0)
                {
                    sb.Append("{}");
                }
                else
                {
                    WriteObjectFields(obj, sb, 0, opts);
                }
            }
            else
            {
                WriteValue(token, sb, 0, opts);
            }

            return sb.ToString().TrimEnd('\n', '\r');
        }

        private static void WriteObjectFields(JObject obj, StringBuilder sb, int level, ToonOptions opts)
        {
            var props = obj.Properties().ToList();
            if (opts.StableKeys) props.Sort((a, b) => string.CompareOrdinal(a.Name, b.Name));
            for (int i = 0; i < props.Count; i++)
            {
                var p = props[i];
                // write key (no colon)
                WriteKeyPrefix(p.Name, sb, level, opts);
                // handle value based on type
                WriteTokenValueByType(p.Value, sb, level + 1, opts, p.Name);
            }
        }

        private static void WriteKeyPrefix(string key, StringBuilder sb, int level, ToonOptions opts)
        {
            Indent(sb, level, opts);
            sb.Append(key);
        }

        // Note: parentKey is optional; signature restored so callers with parentKey compile.
        private static void WriteTokenValueByType(JToken token, StringBuilder sb, int level, ToonOptions opts, string? parentKey = null)
        {
            switch (token.Type)
            {
                case JTokenType.Object:
                    {
                        var obj = (JObject)token;
                        var props = obj.Properties().ToList();
                        if (opts.StableKeys) props.Sort((a, b) => string.CompareOrdinal(a.Name, b.Name));
                        if (props.Count == 0)
                        {
                            // inline empty object: key {} on same line
                            sb.Append(": {}");
                            sb.AppendLine();
                        }
                        else
                        {
                            sb.AppendLine(":");
                            WriteObjectFields(obj, sb, level, opts);
                        }
                    }
                    break;
                case JTokenType.Array:
                    {
                        var arr = (JArray)token;
                        // key[count]:
                        sb.Append("[");
                        sb.Append(arr.Count);
                        sb.Append("]");
                        sb.AppendLine(":");
                        WriteArray(arr, sb, level, opts);
                    }
                    break;
                default:
                    // primitive inline value: key: value
                    sb.Append(": ");
                    WriteValue(token, sb, level, opts);
                    sb.AppendLine();
                    break;
            }
        }

        private static void WriteArray(JArray arr, StringBuilder sb, int level, ToonOptions opts)
        {
            if (arr.Count == 0)
            {
                Indent(sb, level, opts);
                sb.AppendLine("[]");
                return;
            }

            bool allPrimitives = arr.All(t => IsPrimitive(t.Type));

            if (allPrimitives)
            {
                for (int i = 0; i < arr.Count; i++)
                {
                    Indent(sb, level, opts);
                    WriteValue(arr[i], sb, level, opts);
                    sb.AppendLine();
                }
                return;
            }

            for (int i = 0; i < arr.Count; i++)
            {
                var el = arr[i];
                Indent(sb, level, opts);
                if (el.Type == JTokenType.Object)
                {
                    var obj = (JObject)el;
                    var props = obj.Properties().ToList();
                    if (opts.StableKeys) props.Sort((a, b) => string.CompareOrdinal(a.Name, b.Name));

                    if (props.Count == 0)
                    {
                        sb.AppendLine("- {}");
                    }
                    else
                    {
                        var first = props[0];
                        sb.Append("- ");
                        sb.Append(first.Name);
                        sb.Append(": ");
                        WriteValue(first.Value, sb, level + 1, opts);
                        sb.AppendLine();

                        for (int j = 1; j < props.Count; j++)
                        {
                            var p = props[j];
                            Indent(sb, level + 1, opts);
                            sb.Append(p.Name);
                            sb.Append(": ");
                            WriteValue(p.Value, sb, level + 1, opts);
                            sb.AppendLine();
                        }
                    }
                }
                else if (el.Type == JTokenType.Array)
                {
                    sb.Append("- ");
                    var nested = (JArray)el;
                    sb.Append("[");
                    sb.Append(nested.Count);
                    sb.Append("]:");
                    sb.AppendLine();
                    WriteArray(nested, sb, level + 1, opts);
                }
                else
                {
                    sb.Append("- ");
                    WriteValue(el, sb, level + 1, opts);
                    sb.AppendLine();
                }
            }
        }

        private static void WriteValue(JToken token, StringBuilder sb, int level, ToonOptions opts)
        {
            switch (token.Type)
            {
                case JTokenType.String:
                    sb.Append(EscapeString(token.Value<string?>(), opts));
                    break;
                case JTokenType.Integer:
                case JTokenType.Float:
                case JTokenType.Boolean:
                    sb.Append(token.ToString(Newtonsoft.Json.Formatting.None));
                    break;
                case JTokenType.Null:
                    sb.Append("null");
                    break;
                case JTokenType.Object:
                    sb.AppendLine();
                    WriteObjectFields((JObject)token, sb, level + 1, opts);
                    break;
                case JTokenType.Array:
                    var arr = (JArray)token;
                    sb.Append("[");
                    sb.Append(arr.Count);
                    sb.Append("]");
                    break;
                default:
                    sb.Append(EscapeString(token.ToString(), opts));
                    break;
            }
        }

        private static bool IsPrimitive(JTokenType t)
        {
            return t == JTokenType.String || t == JTokenType.Integer || t == JTokenType.Float || t == JTokenType.Boolean || t == JTokenType.Null;
        }

        private static void Indent(StringBuilder sb, int level, ToonOptions opts)
        {
            int spaces = opts.Indent <= 0 ? level * 2 : level * opts.Indent;
            sb.Append(new string(' ', spaces));
        }

        // accept nullable string to suppress possible-null warnings
        private static string EscapeString(string? s, ToonOptions opts)
        {
            if (s == null) return "\"\"";
            if (!opts.SafeStrings)
            {
                return s.Length == 0 ? "\"\"" : s;
            }

            bool needsQuote = s.Length == 0
                || s.Any(c => char.IsWhiteSpace(c))
                || s.IndexOfAny(new[] { ':', '{', '}', '[', ']', ',', '"' }) >= 0;

            if (!needsQuote) return s;
            return Newtonsoft.Json.JsonConvert.ToString(s);
        }
    }
}
