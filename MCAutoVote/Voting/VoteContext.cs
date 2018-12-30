using MCAutoVote.CLI;
using OpenQA.Selenium.Remote;

namespace MCAutoVote.Voting
{
    public class VoteContext
    {
        public string Nickname { get; }
        public RemoteWebDriver Driver { get; }

        public VoteContext(string nickname, RemoteWebDriver driver)
        {
            Nickname = nickname;
            Driver = driver;
        }

        public void Log(string str, params object[] parameters)
        {
            CLIOutput.WriteLine("    " + str, System.ConsoleColor.Gray, parameters);
        }
    }
}
