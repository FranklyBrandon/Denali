using Denali.App.Widgets;
using Denali.App.Widgets.HistoricAnalysis;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace Denali.App.Main
{
    public class MainViewModel : ViewModelBase
    {
        private ContentControl _currentStockWidget;
        public ContentControl CurrentStockWidget { get => _currentStockWidget; set => Set(ref _currentStockWidget, value); }
        public IDictionary<string, ContentControl> StockWidgets { get; set; }

        public MainViewModel()
        {
            this.StockWidgets = new Dictionary<string, ContentControl>();
        }
        public void OnStockAdded(object sender, string symbol)
        {
            if (!StockWidgets.ContainsKey(symbol))
            {
                var widget = new HistoricAnalysisWidget();
                var widgetViewModel = new HistoricAnalysisViewModel();
                widget.DataContext = widgetViewModel;
                widgetViewModel.Symbol = symbol;

                StockWidgets.Add(symbol, widget);
                CurrentStockWidget = StockWidgets[symbol];
            }         
        }
    }
}
