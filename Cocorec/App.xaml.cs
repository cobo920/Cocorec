using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Cocorec.ViewModel;
using Cocorec.Model;

namespace Cocorec
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var window = new MainWindow();
            window.DataContext = new RecorderViewModel(new RecorderModel());
            window.Show();
        }
    }
}
