using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace AllInOnePlayer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            AllInOnePlayer.Properties.Settings.Default.Save();
            GridLength n = AllInOnePlayer.Properties.Settings.Default.MyColumnWidthSetting;
        }
    }
}
