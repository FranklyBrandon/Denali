import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class StockDataService {

  constructor(private http: HttpClient) { }

  public getStockAggregateData(ticker: string) : Observable<any> {
    return this.http.get(`https://localhost:7016/stocks/${ticker}/aggregate?startDateTime=2023-04-06T14%3A30%3A00Z&endDateTime=2023-04-06T21%3A00%3A00Z&timeFrame=1`)
  }
}
