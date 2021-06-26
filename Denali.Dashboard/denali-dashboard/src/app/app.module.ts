import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { StoreModule } from '@ngrx/store';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { StockAlertComponent } from './dashboard/stock-alert/stock-alert.component';
import { appReducers } from './store/reducers/app.reducer';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';

import { CardModule } from 'primeng/card';
import {ScrollPanelModule} from 'primeng/scrollpanel';
import { NavMenuComponent } from './dashboard/menu/nav-menu/nav-menu.component';
import {TabMenuModule} from 'primeng/tabmenu';
import {MenuItem} from 'primeng/api';

@NgModule({
  declarations: [
    AppComponent,
    StockAlertComponent,
    NavMenuComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    AppRoutingModule,
    CardModule,
    ScrollPanelModule,
    TabMenuModule,
    StoreModule.forRoot(appReducers)
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
