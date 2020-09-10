using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Text;

namespace Denali.App.Widgets
{
    public class WidgetViewModelBase : ViewModelBase
    {
        private string _symbol;
        public string Symbol { get => _symbol; set => Set(ref _symbol, value); }
    }
}
