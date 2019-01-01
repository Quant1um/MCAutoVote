using MCAutoVote.Preferences.Reflection;
using System;
using System.Reflection;

namespace MCAutoVote.Preferences
{
    public class StringAccessor : IAccessor
    {
        public IAccessor WrappedAccessor { get; }
        public Type Type { get; }
        public StringConverter Converter { get; }

        public StringAccessor(MemberInfo member)
        {
            WrappedAccessor = Accessors.CreateAccessor(member);
            
            switch(member.MemberType)
            {
                case MemberTypes.Field: Type = ((FieldInfo)member).FieldType; break;
                case MemberTypes.Property: Type = ((PropertyInfo)member).PropertyType; break;
                default: throw new NotSupportedException("cannot be thrown");
            }

            Converter = StringConverter.Get(Type);
        }
        
        public object Get(object obj)
        {
            return Converter.ToString(WrappedAccessor.Get(obj));
        }

        public void Set(object obj, object value)
        {
            WrappedAccessor.Set(obj, Converter.FromString((string)value));
        }
    }
}