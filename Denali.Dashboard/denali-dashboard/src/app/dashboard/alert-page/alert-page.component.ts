import { Component, OnInit } from '@angular/core';
import { select, Store } from '@ngrx/store';
import { Observable } from 'rxjs';
import { WebsocketConnectionService } from 'src/app/core/websocket-connection.service';
import { selectEntryAlerts } from 'src/app/store/selector/alert.selector';
import { IAppState } from 'src/app/store/state/state.model';

@Component({
  selector: 'app-alert-page',
  templateUrl: './alert-page.component.html',
  styleUrls: ['./alert-page.component.scss']
})
export class StockAlertComponent implements OnInit {



  constructor(
    private readonly websocketService: WebsocketConnectionService) { }

  ngOnInit(): void {

  }

  public OnConnectButton() {
    this.websocketService.EnsureConnected();
  }

}
