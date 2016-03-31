using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Type;

using SC2Statistics.SC2Domain.Model;

namespace SC2Statistics.SC2Domain.Repository.Maps
{
    public class PlayerMap : ClassMapping<Player>
    {
        public PlayerMap()
        {
            Table("Players");
            Lazy(false);

            Id(x => x.Id, mapper => mapper.Generator(Generators.Identity));

            Property(x => x.AligulacId, mapper => mapper.NotNullable(true));
            Property(x => x.AligulacReferenceUrl, mapper => mapper.NotNullable(true));
            Property(x => x.Birthday, mapper => mapper.NotNullable(false));
            Property(x => x.Country, mapper => mapper.NotNullable(false));
            Property(x => x.LiquipediaName, mapper => mapper.NotNullable(false));
            Property(x => x.Name, mapper => mapper.NotNullable(false));
            Property(x => x.Race, mapper => { mapper.NotNullable(true); mapper.Type<EnumStringType<Race>>(); });
            Property(x => x.Tag, mapper => mapper.NotNullable(true));
        }
    }
}
