import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { StockDataService } from './stock-data.service';
import { tap } from 'rxjs';

@Component({
  selector: 'app-stock-chart',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './stock-chart.component.html',
  styleUrl: './stock-chart.component.scss'
})
export class StockChartComponent {

  public StockSearchInput: string = '';

  constructor(private stockDataService: StockDataService) {}

  public SearchStock() {
    console.log('Fetching stock')
    this.stockDataService.getStockAggregateData("VTI")
      .subscribe({
        next: (v) => console.log(v),
        error: (e) => console.error(e),
        complete: () => console.info('complete') 
      })
  }
}
