using MCAutoVote.Bootstrap;
using System.Collections.Generic;
using static MCAutoVote.Utilities.Persistency.PersistentContainer;

namespace MCAutoVote.Utilities.Persistency
{
    public static class Preferences
    {
        private static readonly PersistentContainer container = new PersistentContainer(Info.FilePreferences, new Entry[] {
            new Entry("hidden",     typeof(bool),   false),
            new Entry("autovote",   typeof(bool),   true),
            new Entry("nickname",   typeof(string), null)
        });

        public static bool Hidden
        {
            get => (bool) container["hidden"];
            set => container["hidden"] = value;
        }

        public static string Nickname
        {
            get => (string) container["nickname"];
            set => container["nickname"] = value;
        }

        public static IEnumerable<Property> Enumerate()
        {
            foreach (Property prop in container)
                yield return prop;
        }
    }
}
