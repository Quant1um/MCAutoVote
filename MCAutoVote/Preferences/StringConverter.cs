using MCAutoVote.Web;
using System;
using System.Collections.Generic;

namespace MCAutoVote.Preferences
{
    public abstract class StringConverter
    {
        private static IDictionary<Type, StringConverter> Converters { get; } = new Dictionary<Type, StringConverter>()
        {
            [typeof(string)] = new StringIdentityConverter(),
            [typeof(bool)] = new BooleanConverter(),
            [typeof(BrowserDriverInfo)] = new BrowserDriverInfoConverter()
        };

        public static StringConverter Get(Type type)
        {
            if(Converters.TryGetValue(type, out StringConverter converter))
                return converter;
            throw new NotSupportedException("Unsupported type: " + type.Name);
        }

        public abstract string ToString(object obj);
        public abstract object FromString(string str);

        private class StringIdentityConverter : StringConverter
        {
            public override object FromString(string str) => str;
            public override string ToString(object obj) => (string)obj;
        }

        private class BooleanConverter : StringConverter
        {
            private static IDictionary<string, bool> StringBooleanMap { get; } = new Dictionary<string, bool>()
            {
                ["+"]           = true,
                ["-"]           = false,
                ["1"]           = true,
                ["0"]           = false,
                ["t"]           = true,
                ["f"]           = false,
                ["on"]          = true,
                ["off"]         = false,
                ["true"]        = true,
                ["false"]       = false,
                ["enable"]      = true,
                ["disable"]     = false,
                ["enabled"]     = true,
                ["disabled"]    = false
            };

            public override object FromString(string str)
            {
                if (StringBooleanMap.TryGetValue(str.ToLower(), out bool value))
                    return value;
                throw new ArgumentException("Undefined boolean keyword!");
            }

            public override string ToString(object obj)
            {
                if ((bool)obj == true)
                    return "true";
                return "false";
            }
        }

        private class BrowserDriverInfoConverter : StringConverter
        {
            public override object FromString(string str) {
                if (str == null) return null;
                return BrowserDriverInfo.Parse(str);
            }

            public override string ToString(object obj)
            {
                if (obj == null) return null;
                return obj.ToString();
            }
        }
    }
}
