using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Globalization;

namespace Tililaskuri
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {            
            Tililaskuri.Properties.Resources.Culture = new CultureInfo(Tililaskuri.Properties.Settings.Default.Kieli);
        }
    }
}
