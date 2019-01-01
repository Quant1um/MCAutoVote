using System;

namespace MCAutoVote.Preferences.Editor
{
    public class EditablePreferenceAttribute : Attribute {
        public bool CanUnset { get; set; } = false;
    }
}