using Denali.App.StockExplorer;
using Denali.App.Widgets.HistoricAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Denali.App.Widgets
{
    public class WidgetFactory
    {
        public HistoricAnalysisWidget CreateHistoricAnalysisWidget()
        {
            return new HistoricAnalysisWidget();
        }

        public StockExplorer.StockExplorer CreateStockExplorer()
        {
            return new StockExplorer.StockExplorer();
        }
    }
}
