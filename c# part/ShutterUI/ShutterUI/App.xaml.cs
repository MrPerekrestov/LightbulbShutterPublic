using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ShutterUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string[] Args;
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            SplashScreen screen = new SplashScreen("icons/logo-FZU-velke.png");            
            screen.Show(true);
            Thread.Sleep(1000);
            if (e.Args.Length > 0)
            {
                Args = e.Args;
            }
        }
    }

}
