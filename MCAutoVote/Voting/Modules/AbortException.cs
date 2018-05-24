using System;

namespace MCAutoVote.Voting.Modules
{
    public class AbortException : Exception
    {
        public AbortException(string message) : base(message) { }
    }
}
