using System;
using System.Windows;
using System.Windows.Media;

using FirstFloor.ModernUI.Presentation;

using SC2LiquipediaStatistics.DesktopClient.Service;
using SC2LiquipediaStatistics.DesktopClient.View;
using SC2LiquipediaStatistics.DesktopClient.ViewModel;
using SC2LiquipediaStatistics.Utilities.Unity;

namespace SC2LiquipediaStatistics.DesktopClient
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
