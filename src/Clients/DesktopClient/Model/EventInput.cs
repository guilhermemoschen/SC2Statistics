using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SC2Statistics.SC2Domain.Service;

namespace SC2LiquipediaStatistics.DesktopClient.Model
{
    public class EventInput : ValidatableObject
    {
        private string liquipediaUrl;
        [RegularExpression(ParseService.ValidateLiquipediaUrl, ErrorMessage = "The provided address is invalid.")]
        [Required(ErrorMessage = "The Liquipedia Url is mandatory.")]
        public string LiquipediaUrl
        {
            get
            {
                return liquipediaUrl;
            }
            set
            {
                if (liquipediaUrl == value)
                    return;

                ValidateAndSet(() => LiquipediaUrl, ref liquipediaUrl, value);
            }
        }
    }
}
