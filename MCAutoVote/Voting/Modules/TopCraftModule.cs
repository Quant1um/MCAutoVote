namespace MCAutoVote.Voting.Modules
{
    public class TopCraftModule : AbstractTCBasedModule
    {
        public TopCraftModule(int projectId) : base(projectId) { }

        public override string Name => "topcraft.ru";
        protected override string Url => $"https://topcraft.ru/servers/{ProjectId}/";
    }
}
