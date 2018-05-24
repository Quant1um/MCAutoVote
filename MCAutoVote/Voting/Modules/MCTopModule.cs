namespace MCAutoVote.Voting.Modules
{
    public class MCTopModule : AbstractTCBasedModule
    {
        public MCTopModule(int projectId) : base(projectId) { }

        public override string Name => "mctop.su";
        protected override string Url => $"https://mctop.su/servers/{ProjectId}/";
    }
}
