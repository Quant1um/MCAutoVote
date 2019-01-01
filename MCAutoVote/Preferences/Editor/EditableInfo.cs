using MCAutoVote.Preferences.Reflection;
using System.Linq;
using System.Reflection;

namespace MCAutoVote.Preferences.Editor
{
    public class EditableInfo
    {
        public string Name { get; }
        public IAccessor Accessor { get; }
        public bool CanUnset { get; }

        public EditableInfo(PropertyInfo f)
        {
            Name = f.Name.ToLower();
            Accessor = new StringAccessor(f);
            CanUnset = ((EditablePreferenceAttribute)f.GetCustomAttributes(typeof(EditablePreferenceAttribute), false).First()).CanUnset;
        }
    }
}