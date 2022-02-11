using Alpaca.Markets;
using Denali.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denali.Processors.BarOverBar
{
    public class BarOverBarAnalysis
    {
        private readonly FileService _fileService;

        public BarOverBarAnalysis(FileService fileService)
        {
            this._fileService = fileService;
        }

        public void Process()
        {

        }
    }
}
