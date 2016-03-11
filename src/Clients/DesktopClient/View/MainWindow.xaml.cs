using System;
using System.Windows.Input;

using FirstFloor.ModernUI.Windows.Controls;

using GalaSoft.MvvmLight.CommandWpf;

using SC2LiquipediaStatistics.DesktopClient.Service;
using SC2LiquipediaStatistics.Utilities.Unity;

namespace SC2LiquipediaStatistics.DesktopClient.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            NavigateToHomeView();
        }

        private void NavigateToHomeView()
        {
            var navigationService = Container.Resolve<IModernNavigationService>();
            ContentSource = navigationService.GetPageUri(ViewLocator.InitialView);
        }
    }
}
