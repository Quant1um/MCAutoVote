namespace MCAutoVote.Preferences.Reflection
{
    public interface IAccessor
    {
        void Set(object obj, object value);
        object Get(object obj);
    }
}
