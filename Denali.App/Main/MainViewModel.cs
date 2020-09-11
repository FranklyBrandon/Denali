using Denali.App.Widgets;
using Denali.App.Widgets.HistoricAnalysis;
using Denali.Services;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace Denali.App.Main
{
    public class MainViewModel : ViewModelBase
    {
        private readonly DenaliAppServices _appServices;

        private ContentControl _currentStockWidget;
        public ContentControl CurrentStockWidget { get => _currentStockWidget; set => Set(ref _currentStockWidget, value); }
        public IDictionary<string, ContentControl> StockWidgets { get; set; }

        public MainViewModel(DenaliAppServices appServices)
        {
            this._appServices = appServices;

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
