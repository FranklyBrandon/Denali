import { Routes } from '@angular/router';
import { StockChartComponent } from './stock-chart/stock-chart.component';

export const routes: Routes = [
    {path: '', pathMatch: 'full', redirectTo: 'stock'},
    {path: 'stock', component: StockChartComponent}
];
