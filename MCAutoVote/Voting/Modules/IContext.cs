using MCAutoVote.Web;

namespace MCAutoVote.Voting.Modules
{
    public interface IContext
    {
        string Nickname { get; }
        IBrowser Browser { get; }
    }
}
