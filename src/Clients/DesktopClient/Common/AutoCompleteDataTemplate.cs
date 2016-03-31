using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SC2LiquipediaStatistics.DesktopClient.Common
{
    public class AutoCompleteDataTemplate : DataTemplateSelector
    {
        public DataTemplate AutoCompleteTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return AutoCompleteTemplate ?? base.SelectTemplate(item, container);
        }
    }
}
