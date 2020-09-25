using Denali.App.Main;
using Denali.App.StockExplorer;
using Denali.App.Widgets;
using Denali.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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

namespace Denali.App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(WidgetFactory widgetFactory)
        {
            InitializeComponent();
            var mainViewModel = new MainViewModel(widgetFactory);
            this.DataContext = mainViewModel;

            var stockExplorerViewModel = new StockExplorerViewModel();
            stockExplorerViewModel.StockAddedEvent += mainViewModel.OnStockAdded;
            this.StockExplorer.Content = widgetFactory.CreateStockExplorer();
            this.StockExplorer.DataContext = stockExplorerViewModel;
        }
    }
}
