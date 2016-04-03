using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using FirstFloor.ModernUI.Windows.Controls;

namespace SC2LiquipediaStatistics.DesktopClient.Common
{
    public static class Dialog
    {
        public static MessageBoxResult ShowMessage(string text, string title, MessageBoxButton button, Window owner = null)
        {
            var dlg = new ModernDialog
            {
                Title = title,
                Content = new BBCodeBlock { BBCode = text, Margin = new Thickness(0, 0, 0, 8) },
                MinHeight = 0,
                MinWidth = 0,
                MaxHeight = 480,
                MaxWidth = 640,
                BackgroundContent = null,
            };
            if (owner != null)
            {
                dlg.Owner = owner;
            }

            dlg.Buttons = GetButtons(dlg, button);
            dlg.ShowDialog();
            return dlg.MessageBoxResult;
        }

        private static IEnumerable<Button> GetButtons(ModernDialog owner, MessageBoxButton button)
        {
            if (button == MessageBoxButton.OK)
            {
                yield return owner.OkButton;
            }
            else if (button == MessageBoxButton.OKCancel)
            {
                yield return owner.OkButton;
                yield return owner.CancelButton;
            }
            else if (button == MessageBoxButton.YesNo)
            {
                yield return owner.YesButton;
                yield return owner.NoButton;
            }
            else if (button == MessageBoxButton.YesNoCancel)
            {
                yield return owner.YesButton;
                yield return owner.NoButton;
                yield return owner.CancelButton;
            }
        }
    }
}
