using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NightSwitcher.Helpers
{
    public class RegistryHelper
    {
        public static void ToggleTheme()
        {
            var rk = Registry.CurrentUser.OpenSubKey(Constants.PersonalizeLocation, true);

            rk.SetValue(Constants.AppsUseLightTheme, !Convert.ToBoolean((int)rk.GetValue(Constants.AppsUseLightTheme)), RegistryValueKind.DWord);
            rk.Close();
        }

        public static void ToggleTheme(bool useLightTheme)
        {
            var rk = Registry.CurrentUser.OpenSubKey(Constants.PersonalizeLocation, true);

            rk.SetValue(Constants.AppsUseLightTheme, useLightTheme, RegistryValueKind.DWord);
            rk.Close();
        }

        public static bool IsLightModeOn()
        {
            var rk = Registry.CurrentUser.OpenSubKey(Constants.PersonalizeLocation, false);

            return Convert.ToBoolean((int)rk.GetValue(Constants.AppsUseLightTheme));
        }

        public static void RunAtStartup(bool runAtStartup)
        {
            var rk = Registry.CurrentUser.OpenSubKey(Constants.StartupLocation, true);
            
            if (runAtStartup)
            {
                var location = AppDomain.CurrentDomain.BaseDirectory;
                rk.SetValue(Constants.NightSwitcher, $"\"{location}{Constants.NightSwitcher}.exe\" /background");
            }
            else
            {
                rk.DeleteValue(Constants.NightSwitcher);
            }

            rk.Close();
        }

        public static bool IsRunningAtStartup()
        {
            var rk = Registry.CurrentUser.OpenSubKey(Constants.StartupLocation, false);

            return rk.GetValue(Constants.NightSwitcher) is not null;
        }
    }
}
