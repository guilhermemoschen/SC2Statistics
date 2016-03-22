using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

using SC2Statistics.SC2Domain.Model;

namespace SC2LiquipediaStatistics.DesktopClient.Common
{
    public class RaceToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return new SolidColorBrush(Colors.Transparent);
            }

            var race = ConvertToRace(value);

            switch (race)
            {
                case Race.Terran:
                    return new SolidColorBrush(Color.FromRgb(184, 184, 242));

                case Race.Protoss:
                    return new SolidColorBrush(Color.FromRgb(184, 242, 184));

                case Race.Zerg:
                    return new SolidColorBrush(Color.FromRgb(242, 184, 184));

                case Race.Random:
                    return new SolidColorBrush(Color.FromRgb(242, 232, 184));

                default:
                    return new SolidColorBrush(Colors.Transparent);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public Race ConvertToRace(object o)
        {
            if (o.GetType() != typeof(Race))
                return Race.Undefined;
            return (Race)Enum.Parse(typeof(Race), o.ToString());
        }
    }
}
