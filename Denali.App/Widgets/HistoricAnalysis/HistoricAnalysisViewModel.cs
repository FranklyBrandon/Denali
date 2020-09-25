using Denali.Models;
using Denali.Services.Analysis;
using Denali.Services.Utility;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Text;
using System.Windows.Input;

namespace Denali.App.Widgets.HistoricAnalysis
{
    public class HistoricAnalysisViewModel : WidgetViewModelBase
    {

        public ICommand StartAnalysisCommand { get; set; }

        private HistoricAnalysisService _historicAnalysisService;
        private TimeUtils _timeUtils;
        private string _fromTime;
        public string FromTime { get => _fromTime; set => Set(ref _fromTime, value); }
        private string _toTime;
        public string ToTime { get => _toTime; set => Set(ref _toTime, value); }

        public DateTime FromDate { get => _fromDate; set => Set(ref _fromDate, value); }
        private DateTime _fromDate;

        public DateTime ToDate { get => _toDate; set => Set(ref _toDate, value); }
        private DateTime _toDate;

        public HistoricAnalysisViewModel(HistoricAnalysisService historicAnalysisService, TimeUtils timeUtils)
        {
            _historicAnalysisService = historicAnalysisService;
            _timeUtils = timeUtils;

            FromTime = "09:30";
            ToTime = "16:00";
            FromDate = DateTime.Today;
            ToDate = DateTime.Today;

            StartAnalysisCommand = new RelayCommand(OnStartAnalysisClick);
        }

        private async void OnStartAnalysisClick()
        {
            var fromTime = GetTime(FromTime);
            var toTime = GetTime(ToTime);
            var fromDate = new DateTimeWithZone(FromDate.Add(fromTime), _timeUtils.EasternStandardTime);
            var toDate = new DateTimeWithZone(ToDate.Add(toTime), _timeUtils.EasternStandardTime);

            _historicAnalysisService.RunHistoricAnalysis(Symbol, fromDate, toDate, Models.Data.FinnHub.CandleResolution.One);
        }

        private TimeSpan GetTime(string time)
        {
            DateTime dt;
            if (!DateTime.TryParseExact(time, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
            {
                // handle validation error
            }
            return dt.TimeOfDay;
        }
    }
}