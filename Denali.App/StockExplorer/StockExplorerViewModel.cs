using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Denali.App.StockExplorer
{
    public class StockExplorerViewModel : ViewModelBase
    {
        private string _inputStock;
        public string InputStock { get => _inputStock; set => Set(ref _inputStock, value); }


        private ObservableCollection<StockSelection> _stockSelections = new ObservableCollection<StockSelection>();
        public ObservableCollection<StockSelection> StockSelections { get => _stockSelections; set => Set(ref _stockSelections, value); }

        public ICommand AddStockCommand => _addStockCommand;
        private ICommand _addStockCommand { get; }

        public StockExplorerViewModel()
        {
            _addStockCommand = new RelayCommand(AddStockSelection);
        }

        private void AddStockSelection()
        {
            this.StockSelections.Add(new StockSelection { Symbol = this.InputStock });
            this.InputStock = string.Empty;
        }
    }
}
