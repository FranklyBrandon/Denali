import { Component, OnInit } from '@angular/core';
import { WebsocketConnectionService } from 'src/app/core/websocket-connection.service';

@Component({
  selector: 'app-stock-alert',
  templateUrl: './stock-alert.component.html',
  styleUrls: ['./stock-alert.component.css']
})
export class StockAlertComponent implements OnInit {

  constructor(
    private readonly websocketService: WebsocketConnectionService
    ) { }

  ngOnInit(): void {
  }

  public OnConnectButton() {
    this.websocketService.EnsureConnected();
  }

}
