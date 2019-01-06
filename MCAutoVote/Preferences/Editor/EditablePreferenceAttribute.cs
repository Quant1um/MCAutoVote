using System;

namespace MCAutoVote.Preferences.Editor
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class EditablePreferenceAttribute : Attribute {
        public bool CanUnset { get; set; } = false;
        public string Name { get; set; } = null;
    }
}