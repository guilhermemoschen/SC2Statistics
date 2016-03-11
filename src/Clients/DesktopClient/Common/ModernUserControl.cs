using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

using FirstFloor.ModernUI.Windows;
using FirstFloor.ModernUI.Windows.Navigation;

using SC2LiquipediaStatistics.DesktopClient.Service;
using SC2LiquipediaStatistics.DesktopClient.ViewModel;
using SC2LiquipediaStatistics.Utilities.Unity;

namespace SC2LiquipediaStatistics.DesktopClient.Common
{
    /// <summary>
    /// Class ModernUserControl.
    /// </summary>
    public class ModernUserControl : UserControl, IContent
    {
        private IModernNavigationService navigationService;

        /// <summary>
        /// Creates an instance of the country repository or returns the already constructed one.
        /// </summary>
        public IModernNavigationService NavigationService
        {
            get
            {
                return navigationService ?? (navigationService = Container.Resolve<IModernNavigationService>());
            }
            set
            {
                navigationService = value;
            }
        }

        /// <summary>
        /// Handles the <see cref="E:FragmentNavigation"/> event.
        /// </summary>
        /// <param name="e">The <see cref="FirstFloor.ModernUI.Windows.Navigation.FragmentNavigationEventArgs"/> instance containing the event data.</param>
        public void OnFragmentNavigation(FragmentNavigationEventArgs e)
        {
            var viewModel = DataContext as ModernViewModelBase;
            if (viewModel != null && viewModel.FragmentNavigationCommand != null)
            {
                viewModel.FragmentNavigationCommand.Execute(null);
            }
        }

        /// <summary>
        /// Handles the <see cref="E:NavigatedFrom"/> event.
        /// </summary>
        /// <param name="e">The <see cref="FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs"/> instance containing the event data.</param>
        public void OnNavigatedFrom(NavigationEventArgs e)
        {
            var viewModel = DataContext as ModernViewModelBase;
            if (viewModel != null && viewModel.NavigatedFromCommand != null)
            {
                viewModel.NavigatedFromCommand.Execute(null);
            }
        }

        /// <summary>
        /// Handles the <see cref="E:NavigatedTo"/> event.
        /// </summary>
        /// <param name="e">The <see cref="FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs"/> instance containing the event data.</param>
        public void OnNavigatedTo(NavigationEventArgs e)
        {
            var viewModel = DataContext as ModernViewModelBase;
            if (viewModel != null && viewModel.NavigatedToCommand != null)
            {
                viewModel.NavigatedToCommand.Execute(NavigationService.Parameter);
            }
        }

        /// <summary>
        /// Handles the <see cref="E:NavigatingFrom"/> event.
        /// </summary>
        /// <param name="e">The <see cref="FirstFloor.ModernUI.Windows.Navigation.NavigatingCancelEventArgs"/> instance containing the event data.</param>
        public void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            var viewModel = DataContext as ModernViewModelBase;
            if (viewModel != null && viewModel.NavigatingFromCommand != null)
            {
                viewModel.NavigatingFromCommand.Execute(e);
            }
        }

        /// <summary>
        /// Occurs when [navigating from].
        /// </summary>
        public event NavigatingCancelHandler NavigatingFrom;

        /// <summary>
        /// Occurs when [navigated from].
        /// </summary>
        public event NavigationEventHandler NavigatedFrom;

        /// <summary>
        /// Occurs when [navigated to].
        /// </summary>
        public event NavigationEventHandler NavigatedTo;

        /// <summary>
        /// Occurs when [fragment navigation].
        /// </summary>
        public event FragmentNavigationHandler FragmentNavigation;
    }

    /// <summary>
    /// Delegate NavigatingCancelHandler.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="FirstFloor.ModernUI.Windows.Navigation.NavigatingCancelEventArgs"/> instance containing the event data.</param>
    public delegate void NavigatingCancelHandler(object sender, NavigatingCancelEventArgs e);

    /// <summary>
    /// Delegate NavigationEventHandler.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs"/> instance containing the event data.</param>
    public delegate void NavigationEventHandler(object sender, NavigationEventArgs e);

    /// <summary>
    /// Delegate FragmentNavigationHandler.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="FirstFloor.ModernUI.Windows.Navigation.FragmentNavigationEventArgs"/> instance containing the event data.</param>
    public delegate void FragmentNavigationHandler(object sender, FragmentNavigationEventArgs e);
}
