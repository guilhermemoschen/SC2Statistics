using GalaSoft.MvvmLight;

namespace SC2LiquipediaStatistics.DesktopClient.Model
{
    public class Game : ObservableObject
    {
        public long Id { get; set; }
        public string Map { get; set; }
        public int Number { get; set; }
        public Player Winner { get; set; }
        public Match Match { get; set; }
    }
}
