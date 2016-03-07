namespace SC2Statistics.SC2Domain.Model
{
    public class Game
    {
        public virtual long Id { get; set; }
        public virtual string Map { get; set; }
        public virtual int Number { get; set; }
        public virtual Player Winner { get; set; }
        public virtual Match Match { get; set; }
    }
}
