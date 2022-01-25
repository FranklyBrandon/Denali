import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { StockAlertComponent } from './dashboard/alert-page/alert-page.component';

const routes: Routes = [
  { path: '', redirectTo: 'alert', pathMatch: 'full' },
  { path: 'alert', component: StockAlertComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
