using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using static MCAutoVote.Utilities.Persistency.PersistentContainer;

namespace MCAutoVote.Utilities.Persistency
{
    public class PersistentContainer : IEnumerable<Property>
    {
        public class Entry
        {
            public string Key { get; }
            public Type Type { get; }
            public object Default { get; }

            public Entry(string key, Type type, object def)
            {
                Key = key ?? throw new ArgumentNullException("Key cannot be null!");
                if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("Key cannot be empty or whitespace.");
                
                Type = type ?? throw new ArgumentNullException("Type cannot be null!");

                if(!CheckIsTypeValid(def)) throw new ArgumentNullException("Default object is not valid (not assignable to given type)!");
                Default = def;
            }

            public bool CheckIsTypeValid(object obj) =>
                obj == null ? Type.CanBeNull() : Type.IsInstanceOfType(obj);
        }

        public class Property
        {
            public Entry Entry { get; }

            private object _value;
            public object Value
            {
                get => _value;
                set {
                    if(!Entry.CheckIsTypeValid(value))
                        throw new ArgumentNullException("Value object is not valid (not assignable to entry type)");
                    _value = value;
                }
            }

            public string Key => Entry.Key;
            public Type Type => Entry.Type;
            public object Default => Entry.Default;

            public Property(Entry entry)
            {
                Entry = entry;
                _value = Entry.Default;
            }
        }

        public string FilePath { get; }
        private IDictionary<string, Property> container = new Dictionary<string, Property>();
       
        public PersistentContainer(string filePath, IEnumerable<Entry> entries)
        {
            FilePath = filePath;

            foreach (Entry entry in entries)
                container.Add(entry.Key, new Property(entry));

            Load();
        }

        public object this[string str]
        {
            get
            {
                if (container.TryGetValue(str, out Property obj))
                    return obj.Value;
                throw new ArgumentException("Invalid key!");
            }

            set
            {
                if (container.TryGetValue(str, out Property obj))
                {
                    obj.Value = value;
                    Save();
                }else
                    throw new ArgumentException("Invalid key!");
            }
        }

        public void Load()
        {
            if (!File.Exists(FilePath)) return;

            try
            {
                JObject obj = JObject.Parse(File.ReadAllText(FilePath));

                foreach (Property prop in this)
                {
                    string key = prop.Key;
                    Type type = prop.Type;
                    JProperty jProp = obj.Property(prop.Entry.Key);

                    if (jProp != null)
                        prop.Value = jProp.Value.ToObject(type);
                }
            }
            catch (Exception e)
            {
                Interface.Text.WriteLine("Failed to load");
            }
        }

        public void Save()
        {
            try
            {
                JObject obj = new JObject();

                foreach (Property prop in this)
                {
                    string key = prop.Key;
                    Type type = prop.Type;

                    obj.Add(new JProperty(key, prop.Value));
                }

                File.WriteAllText(FilePath, obj.ToString());
            }
            catch (Exception e)
            {
                Interface.Text.WriteLine("Failed to save");
            }
        }

        public IEnumerator<Property> GetEnumerator() =>
            container.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            container.Values.GetEnumerator();
    }
}
