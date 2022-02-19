using Microsoft.Win32;
using NightSwitcher.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace NightSwitcher.Commands
{
    public class ToggleThemeCommand : CommandBase<ToggleThemeCommand>
    {

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            RegistryHelper.ToggleTheme();

            Application.Current.Dispatcher.Invoke(() =>
            {
                var grid = (System.Windows.Controls.Grid)parameter;
                var button = grid.Children.OfType<System.Windows.Controls.Primitives.ToggleButton>().First(x => x.Name == nameof(MainWindow.NightModeBtn));
                var taskBar = grid.Children.OfType<Hardcodet.Wpf.TaskbarNotification.TaskbarIcon>().First();
                button.IsChecked = !button.IsChecked;

                taskBar.Icon = button.IsChecked.Value ?
                    new Icon(Application.GetResourceStream(new Uri(Constants.YellowIconUri)).Stream) :
                    new Icon(Application.GetResourceStream(new Uri(Constants.WhiteIconUri)).Stream);

            }, DispatcherPriority.ContextIdle);

        }
    }
}
