using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using FirstFloor.ModernUI.Windows.Controls;

using NHibernate.Transform;

using SC2LiquipediaStatistics.DesktopClient.View;

namespace SC2LiquipediaStatistics.DesktopClient.Service
{
    public class LoadingService : ILoadingService
    {
        private readonly ModernDialog loadingWindow;
        private Action workAction;
        private readonly FrameworkElement windowContent;
        private bool IsFirstLoad;
        private Exception actionException;

        public LoadingService()
        {
            IsFirstLoad = true;
            windowContent = new LoadingView();
            
            loadingWindow = new ModernDialog
            {
                Title = "Loading...",
                Content = windowContent,
                WindowStyle = WindowStyle.None,
                Buttons = new Button[] { },
                ResizeMode = ResizeMode.NoResize,
                MinHeight = 400,
                MaxHeight = 400,
                MinWidth = 400,
                MaxWidth = 400,
                Owner = Application.Current.MainWindow
            };
        }

        public void ShowAndExecuteAction(Action action)
        {
            actionException = null;
            workAction = action;
            loadingWindow.Loaded += LoadingWindow_Loaded;
            if (!IsFirstLoad)
                loadingWindow.RaiseEvent(new RoutedEventArgs(FrameworkElement.LoadedEvent));
            loadingWindow.ShowDialog();
            loadingWindow.Loaded -= LoadingWindow_Loaded;
            workAction = null;
            if (actionException != null)
                throw actionException;
        }

        private void LoadingWindow_Loaded(object sender, RoutedEventArgs routedEventArgs)
        {
            if (!IsFirstLoad)
                windowContent.RaiseEvent(new RoutedEventArgs(FrameworkElement.LoadedEvent));
            var worker = new BackgroundWorker();
            worker.DoWork += (s, workerArgs) => workAction();
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            worker.RunWorkerAsync();
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs runWorkerCompletedEventArgs)
        {
            IsFirstLoad = false;
            loadingWindow.Hide();
            actionException = runWorkerCompletedEventArgs.Error;
        }
    }
}
