using Denali.App.StockExplorer;
using Denali.App.Widgets.HistoricAnalysis;
using Denali.Services.Analysis;
using Denali.Services.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace Denali.App.Widgets
{
    public class WidgetFactory
    {
        private readonly HistoricAnalysisService _historicAnalysisService;
        private readonly TimeUtils _timeUtils;

        public WidgetFactory(HistoricAnalysisService historicAnalysisService, TimeUtils timeUtils)
        {
            this._historicAnalysisService = historicAnalysisService;
            this._timeUtils = timeUtils;
        }

        public HistoricAnalysisWidget CreateHistoricAnalysisWidget() => new HistoricAnalysisWidget();

        public HistoricAnalysisViewModel CreateHistoricAnalysisViewModel() => new HistoricAnalysisViewModel(_historicAnalysisService, _timeUtils);

        public StockExplorer.StockExplorer CreateStockExplorer() => new StockExplorer.StockExplorer();

        public StockExplorer.StockExplorerViewModel CreateStockExplorerViewModel() => new StockExplorerViewModel();
    }
}
