using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Denali.App.StockExplorer
{
    public class StockExplorerViewModel : ViewModelBase
    {
        #region ViewProperties
        private string _inputStock;
        public string InputStock { get => _inputStock; set => Set(ref _inputStock, value); }

        private ObservableCollection<StockSelection> _stockSelections = new ObservableCollection<StockSelection>();
        public ObservableCollection<StockSelection> StockSelections { get => _stockSelections; set => Set(ref _stockSelections, value); }
        #endregion
        #region Commands
        public ICommand AddStockCommand => _addStockCommand;
        private ICommand _addStockCommand { get; }
        #endregion
        #region Events
        public event EventHandler<string> StockAddedEvent;
        #endregion

        public StockExplorerViewModel()
        {
            _addStockCommand = new RelayCommand(AddStockSelection);
        }

        private void AddStockSelection()
        {
            this.StockSelections.Add(new StockSelection { Symbol = this.InputStock });
            OnAddStock(this.InputStock);
            this.InputStock = string.Empty;
        }

        protected virtual void OnAddStock(string symbol)
        {
            StockAddedEvent?.Invoke(this, symbol);
        }
    }
}
