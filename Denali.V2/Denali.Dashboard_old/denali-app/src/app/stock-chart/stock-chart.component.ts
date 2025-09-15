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

  public StockSearchInput: string = 'VTI';
  public StartTime: string;
  public EndTime: string;

  constructor(private stockDataService: StockDataService) {
    this.StartTime = '';
    this.EndTime = '';
  }

  public SearchStock() {
    this.stockDataService.getStockAggregateData(this.StockSearchInput)
      .subscribe({
        next: (v) => console.log(v),
        error: (e) => console.error(e),
        complete: () => console.info('complete') 
      })
  }

  private getDefaultStartTime(today: Date): Date {
    let startTime = new Date(today);
    startTime.setHours(9);
    startTime.setMinutes(30);
    return startTime;
  }

  private getDefaultEndTime(today: Date): Date {
    let endTime = new Date(today);
    endTime.setHours(16);
    endTime.setMinutes(0);
    return endTime;
  }
}


