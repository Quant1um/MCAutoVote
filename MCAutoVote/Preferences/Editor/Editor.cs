using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MCAutoVote.Preferences.Editor
{
    public class Editor
    {
        private readonly IDictionary<string, EditableInfo> editables = new Dictionary<string, EditableInfo>();
        private readonly object owner;

        public Editor(object owner)
        {
            foreach (PropertyInfo f in owner.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(f => f.GetCustomAttributes(typeof(EditablePreferenceAttribute), false).Any()))
            {
                EditableInfo info = new EditableInfo(f);
                editables.Add(info.Name, info);
            }

            this.owner = owner;
        }

        private EditableInfo GetEditableInfo(string name)
        {
            if (editables.TryGetValue(name.ToLower(), out EditableInfo info))
                return info;
            throw new ArgumentException("No editable preference with name '" + name + "' exists.");
        }

        public string Get(string name)
        {
            return (string)GetEditableInfo(name).Accessor.Get(owner);
        }

        public string Set(string name, string value)
        {
            string old = Get(name);
            GetEditableInfo(name).Accessor.Set(owner, value);
            return old;
        }

        public string Unset(string name)
        {
            if (!GetEditableInfo(name).CanUnset)
                throw new ArgumentException("Can't unset '" + name + "' preference.");
            return Set(name, null);
        }

        public IDictionary<string, string> View()
        {
            IDictionary<string, string> view = new Dictionary<string, string>();

            foreach(EditableInfo info in editables.Values)
                view.Add(info.Name, Get(info.Name));

            return view;
        }
    }
}
