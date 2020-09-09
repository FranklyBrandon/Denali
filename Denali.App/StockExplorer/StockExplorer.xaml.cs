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
        }

        private void TextBox_KeyEnterUpdate(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox tBox = (TextBox)sender;
                tBox.Text = "";
                //DependencyProperty prop = TextBox.TextProperty;

                //BindingExpression binding = BindingOperations.GetBindingExpression(tBox, prop);
                //if (binding != null) { binding.UpdateSource(); }
            }
        }
    }
}
