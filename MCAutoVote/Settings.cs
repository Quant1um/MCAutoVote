namespace MCAutoVote.Properties {
    internal sealed partial class Settings {

        public Settings()
        {
            PropertyChanged += (sender, e) => Save();
        }
    }
}
