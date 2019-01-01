using OpenQA.Selenium.Remote;

namespace MCAutoVote.Voting
{
    public interface IVoteContext
    {
        string Nickname { get; }
        RemoteWebDriver Driver { get; }

        void Log(string str, params object[] parameters);
    }
}
