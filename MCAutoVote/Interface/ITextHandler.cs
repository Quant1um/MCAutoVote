using System;
using System.Runtime.CompilerServices;

namespace MCAutoVote.Interface
{
    public interface ITextHandler
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        void Write(string str, ConsoleColor color, params object[] parameters);
    }
}  
