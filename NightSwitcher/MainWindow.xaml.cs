using Hardcodet.Wpf.TaskbarNotification;
using NightSwitcher.Extensions;
using NightSwitcher.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace NightSwitcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly CancellationTokenSource startTimeCancellationToken = new();
        private readonly CancellationTokenSource endTimeCancellationToken = new();
        private readonly DateTime baseDateTime;

        public MainWindow()
        {
            InitializeComponent();

            var now = DateTime.Now;

            baseDateTime = new(now.Year, now.Month, now.Day);

            var startTimeValue = baseDateTime + TimeSpan.Parse(App.Config[nameof(Config.StartTime)]);
            var endTimeValue = baseDateTime + TimeSpan.Parse(App.Config[nameof(Config.EndTime)]);

            StartTime.Value = startTimeValue;
            EndTime.Value = endTimeValue;

            AutoTurnBtn.IsChecked = Boolean.Parse(App.Config[nameof(Config.AutoTurn)]);

            var isLightModeOn = RegistryHelper.IsLightModeOn();

            NightModeBtn.IsChecked = !isLightModeOn;
            RunOnStartupBtn.IsChecked = RegistryHelper.IsRunningAtStartup();

            NightSwitcher.Icon = isLightModeOn ?
                    new Icon(Application.GetResourceStream(new Uri(Constants.WhiteIconUri)).Stream) :
                    new Icon(Application.GetResourceStream(new Uri(Constants.YellowIconUri)).Stream);

            //these guys are triggered initialy on SfTimePicker_ValueChanged_StartTime & SfTimePicker_ValueChanged_EndTime
            //ScheduleToggleTask(startTimeValue, true, startTimeCancellationToken);
            //ScheduleToggleTask(startTimeValue, true, endTimeCancellationToken);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            NightSwitcher.Dispose();
            base.OnClosed(e);
        }

        private void SfTimePicker_ValueChanged_StartTime(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!this.IsInitialized)
            {
                return;
            }

            var dt = (DateTime)e.NewValue;
            var value = dt.TimeOfDay.ToString();
            App.Config.ForceSetAppSettingValue(nameof(Config.StartTime), value);

            startTimeCancellationToken.Cancel();
            ScheduleToggleTask(dt, false, startTimeCancellationToken);
        }

        private void SfTimePicker_ValueChanged_EndTime(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!this.IsInitialized)
            {
                return;
            }

            var dt = (DateTime)e.NewValue;
            var value = dt.TimeOfDay.ToString();
            App.Config.ForceSetAppSettingValue(nameof(Config.EndTime), value);

            endTimeCancellationToken.Cancel();
            ScheduleToggleTask(dt, true, endTimeCancellationToken);
        }

        private void ScheduleToggleTask(DateTime date, bool useLightTheme, CancellationTokenSource cancellationTokenSource)
        {
            cancellationTokenSource = new();

            var dateNow = DateTime.Now;
            TimeSpan ts;

            if (date > dateNow)
            {
                ts = date - dateNow;
            }
            else
            {
                date = date.AddDays(1);
                ts = date - dateNow;
            }

            Task.Delay(ts).ContinueWith((x) =>
            {
                RegistryHelper.ToggleTheme(useLightTheme);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    NightModeBtn.IsChecked = !useLightTheme;
                }, DispatcherPriority.ContextIdle);
                
                ScheduleToggleTask(date.AddDays(1), useLightTheme, cancellationTokenSource);

            }, cancellationTokenSource.Token);
        }

        private void AutoTurnButton_Checked(object sender, RoutedEventArgs e)
        {
            ScheduleToggleTask(StartTime.Value.Value, true, startTimeCancellationToken);
            ScheduleToggleTask(EndTime.Value.Value, false, endTimeCancellationToken);

            App.Config.ForceSetAppSettingValue(nameof(Config.AutoTurn), true);
        }

        private void AutoTurnButton_Unchecked(object sender, RoutedEventArgs e)
        {
            startTimeCancellationToken.Cancel();
            endTimeCancellationToken.Cancel();

            App.Config.ForceSetAppSettingValue(nameof(Config.AutoTurn), false);
        }

        private void NightModeButton_Checked(object sender, RoutedEventArgs e)
        {
            RegistryHelper.ToggleTheme(false);

            NightSwitcher.Icon = new Icon(Application.GetResourceStream(new Uri(Constants.YellowIconUri)).Stream);
        }

        private void NightModeButton_Unchecked(object sender, RoutedEventArgs e)
        {
            RegistryHelper.ToggleTheme(true);

            NightSwitcher.Icon = new Icon(Application.GetResourceStream(new Uri(Constants.WhiteIconUri)).Stream);
        }
        private void RunAtStartupButton_Checked(object sender, RoutedEventArgs e)
        {
            RegistryHelper.RunAtStartup(true);
        }

        private void RunAtStartupButton_Unchecked(object sender, RoutedEventArgs e)
        {
            RegistryHelper.RunAtStartup(false);
        }
    }
}
