using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SC2LiquipediaStatistics.DesktopClient.Common
{
    public static class Behaviors
    {
        public static readonly DependencyProperty LoadedMethodNameProperty = DependencyProperty.RegisterAttached(
            "LoadedMethodName",
            typeof(string),
            typeof(Behaviors),
            new PropertyMetadata(null, OnLoadedMethodNameChanged)
        );

        private static void OnLoadedMethodNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = d as FrameworkElement;
            if (element == null)
                return;
            element.Loaded += (sender, args) =>
            {
                var viewModel = element.DataContext;
                if (viewModel == null)
                    return;
                var methodInfo = viewModel.GetType().GetMethod(e.NewValue.ToString());
                if (methodInfo == null)
                    return;
                methodInfo.Invoke(viewModel, null);
            };
        }

        public static void SetLoadedMethodName(DependencyObject obj, string value)
        {
            obj.SetValue(LoadedMethodNameProperty, value);
        }
        public static string GetLoadedMethodName(DependencyObject obj)
        {
            return (string)obj.GetValue(LoadedMethodNameProperty);
        }
    }
}
