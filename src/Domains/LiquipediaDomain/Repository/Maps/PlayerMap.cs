using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

using SC2LiquipediaStatistics.LiquipediaDomain.Model;

namespace SC2LiquipediaStatistics.LiquipediaDomain.Repository.Maps
{
    public class PlayerMap : ClassMapping<Player>
    {
        public PlayerMap()
        {
            Table("Players");
            Lazy(false);

            Id(x => x.Id, mapper => mapper.Generator(Generators.Identity));
            Property(x => x.Name, mapper => mapper.NotNullable(true));

            //Bag(x => x.EventsParticipaed, mapper =>
            //    {
            //        mapper.Table("PlayersEvents");
            //        mapper.Cascade(Cascade.All);
            //        mapper.Key(y => y.Column("FK_Player"));
            //    },
            //    relation => relation.ManyToMany(mapper => mapper.Column("FK_Event"))
            //);
        }
    }
}
