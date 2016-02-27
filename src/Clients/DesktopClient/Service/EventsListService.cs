using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using SC2LiquipediaStatistics.DesktopClient.Model;

namespace SC2LiquipediaStatistics.DesktopClient.Service
{
    public class EventsListService : IEventsListService
    {
        public async Task<IEnumerable<EventItem>> GetEventItems()
        {
            string xmlContennt;

            using (var reader = File.OpenText(ConfigurationManager.AppSettings["EventsListPath"]))
            {
                xmlContennt = await reader.ReadToEndAsync();
            }

            var xml = XElement.Parse(xmlContennt);

            return 
                from item in xml.Elements()
                select new EventItem()
                {
                    Selected = Convert.ToBoolean(item.Attribute("selected").Value),
                    URL = item.Value,
                };
        }
    }
}
