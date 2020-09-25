using Denali.Runner;
using Denali.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Denali.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void OnStartup(object sender, StartupEventArgs e)
        {
            var provider = DenaliConfiguration.Startup();
            var mainWindow = provider.GetService<MainWindow>();
            mainWindow.Show();
        }
    }
}
