using System;

namespace MCAutoVote.Utilities
{
    public static class TypeUtils
    {
        //https://stackoverflow.com/questions/1770181/determine-if-reflected-property-can-be-assigned-null
        public static bool CanBeNull(this Type type)
            => !type.IsValueType || (Nullable.GetUnderlyingType(type) != null);
    }
}
