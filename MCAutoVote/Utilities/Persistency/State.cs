using MCAutoVote.Bootstrap;
using System;
using System.Collections.Generic;
using static MCAutoVote.Utilities.Persistency.PersistentContainer;

namespace MCAutoVote.Utilities.Persistency
{
    public static class State
    {
        private static readonly PersistentContainer container = new PersistentContainer(Info.FileState, new Entry[] {
            new Entry("lastAction", typeof(DateTime), DateTime.MinValue)
        });

        public static DateTime LastAction
        {
            get => (DateTime) container["lastAction"];
            set => container["lastAction"] = value;
        }

        public static IEnumerable<Property> Enumerate()
        {
            foreach (Property prop in container)
                yield return prop;
        }
    }
}
