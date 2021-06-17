import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { StoreModule } from '@ngrx/store';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { StockAlertComponent } from './dashboard/stock-alert/stock-alert.component';
import { alertReducer } from './store/store.reducer';

@NgModule({
  declarations: [
    AppComponent,
    StockAlertComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    StoreModule.forRoot({state: alertReducer})
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
