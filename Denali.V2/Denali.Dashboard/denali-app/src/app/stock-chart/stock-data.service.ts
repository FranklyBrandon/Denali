import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class StockDataService {
  private baseURL: string;

  constructor(private http: HttpClient) {
    this.baseURL = `${environment.apiURL}/${environment.stockDataPath}`
   }

  public getStockAggregateData(ticker: string) : Observable<any> {
    return this.http.get(`${this.baseURL}/${ticker}/aggregate?startDateTime=2023-04-06T14%3A30%3A00Z&endDateTime=2023-04-06T21%3A00%3A00Z&timeFrame=1`)
  }
}
