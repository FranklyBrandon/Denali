﻿using Denali.Models.Alpaca;
using Denali.Models.Polygon;
using Denali.Models.Trading;
using System;
using System.Collections.Generic;
using System.Text;
 
namespace Denali.Algorithms.BarAnalysis
{
    public interface IBarSignalAlgorithm
    {
        void Analyze(IList<Bar> barData);
    }
}