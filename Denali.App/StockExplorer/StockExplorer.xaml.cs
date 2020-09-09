using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Denali.App.StockExplorer
{
    /// <summary>
    /// Interaction logic for StockExplorer.xaml
    /// </summary>
    public partial class StockExplorer : UserControl
    {
        public StockExplorer()
        {
            InitializeComponent();
            var viewModel = new StockExplorerViewModel();
            DataContext = viewModel;
        }
    }
}
