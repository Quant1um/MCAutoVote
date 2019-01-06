using MCAutoVote.Preferences.Reflection;
using System;
using System.Linq;
using System.Reflection;

namespace MCAutoVote.Preferences.Editor
{
    public class EditableInfo
    {
        public string Name { get; }
        public bool CanUnset { get; }
        public Type Type { get; }
        public object Default { get; }

        public IAccessor Accessor { get; }
        public IAccessor DirectAccessor { get; }

        public EditableInfo(PropertyInfo f)
        {
            EditablePreferenceAttribute attr = (EditablePreferenceAttribute)f.GetCustomAttributes(typeof(EditablePreferenceAttribute), false).First();

            Accessor = new StringAccessor(f);
            DirectAccessor = Accessors.CreateAccessor(f);

            Name = (attr.Name ?? f.Name).ToLower();
            CanUnset = attr.CanUnset;

            Type = f.PropertyType;
            Default = Type.IsValueType ? Activator.CreateInstance(Type) : null;
        }
    }
}