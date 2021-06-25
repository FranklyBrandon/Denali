import { Component, OnInit } from '@angular/core';
import { select, Store } from '@ngrx/store';
import { Observable } from 'rxjs';
import { WebsocketConnectionService } from 'src/app/core/websocket-connection.service';
import { selectEntryAlerts } from 'src/app/store/selector/alert.selector';
import { IAppState } from 'src/app/store/state/state.model';

@Component({
  selector: 'app-stock-alert',
  templateUrl: './stock-alert.component.html',
  styleUrls: ['./stock-alert.component.css']
})
export class StockAlertComponent implements OnInit {

  entryAlerts$ = this.store.pipe(select(selectEntryAlerts));

  constructor(
    private readonly websocketService: WebsocketConnectionService,
    private store: Store<IAppState>) { }

  ngOnInit(): void {

  }

  public OnConnectButton() {
    this.websocketService.EnsureConnected();
  }

}
