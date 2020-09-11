using Denali.Services.Analysis;
using Denali.Services.FinnHub;
using System;
using System.Collections.Generic;
using System.Text;

namespace Denali.Services
{
    public class DenaliAppServices
    {
        public HistoricAnalysisService HistoricAnalysisService { get; }

        public DenaliAppServices(HistoricAnalysisService historicAnalysisService)
        {
            HistoricAnalysisService = historicAnalysisService;
        }
    }
}
