using System;
using System.Reflection;

namespace MCAutoVote.Preferences.Reflection
{
    public static class Accessors
    {
        public static IAccessor CreateAccessor(MemberInfo member)
        {
            switch(member.MemberType)
            {
                case MemberTypes.Field: return new FieldAccessor((FieldInfo)member);
                case MemberTypes.Property: return new PropertyAccessor((PropertyInfo)member);
                default: throw new NotSupportedException();
            }
        }
    }

    public class PropertyAccessor : IAccessor
    {
        public PropertyInfo Property { get; }

        public PropertyAccessor(PropertyInfo prop)
        {
            Property = prop;
        }

        public object Get(object obj)
        {
            return Property.GetValue(obj, null);
        }

        public void Set(object obj, object value)
        {
            Property.SetValue(obj, value, null);
        }
    }

    public class FieldAccessor : IAccessor
    {
        public FieldInfo Field { get; }

        public FieldAccessor(FieldInfo field)
        {
            Field = field;
        }

        public object Get(object obj)
        {
            return Field.GetValue(obj);
        }

        public void Set(object obj, object value)
        {
            Field.SetValue(obj, value);
        }
    }
}
