using System.Collections.Generic;

namespace SC2Statistics.SC2Domain.Model
{
    public class Player
    {
        public virtual long Id { get; set; }
        public virtual string Name { get; set; }
        public virtual IList<Event> EventsParticipaed { get; set; }

        public Player()
        {
            EventsParticipaed = new List<Event>();
        }
    }
}
