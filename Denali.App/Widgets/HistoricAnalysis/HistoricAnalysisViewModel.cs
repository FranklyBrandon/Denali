using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Denali.App.Widgets.HistoricAnalysis
{
    public class HistoricAnalysisViewModel : WidgetViewModelBase
    {
        public ICommand StartAnalysisCommand { get; set; }


        private string _fromTime;
        public string FromTime { get => _fromTime; set => Set(ref _fromTime, value); }
        private string _toTime;
        public string ToTime { get => _toTime; set => Set(ref _toTime, value); }

        public HistoricAnalysisViewModel()
        {
            FromTime = "09:30";
            ToTime = "16:30";

            StartAnalysisCommand = new RelayCommand(OnStartAnalysisClick);
        }

        private async void OnStartAnalysisClick()
        {

        }
    }
}
