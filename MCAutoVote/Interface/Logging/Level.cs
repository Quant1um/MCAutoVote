namespace MCAutoVote.Interface.Logging
{
    public enum Level : uint
    {
        Echo    = 1 << 0,
        Info    = 1 << 1,
        Verbose = 1 << 2,
        Warning = 1 << 3,
        Error   = 1 << 4
    }
}
