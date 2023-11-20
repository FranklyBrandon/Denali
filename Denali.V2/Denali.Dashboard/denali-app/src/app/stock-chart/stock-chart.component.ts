import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-stock-chart',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './stock-chart.component.html',
  styleUrl: './stock-chart.component.scss'
})
export class StockChartComponent {

  public StockSearchInput: string = '';

  public SearchStock() {

  }
}
