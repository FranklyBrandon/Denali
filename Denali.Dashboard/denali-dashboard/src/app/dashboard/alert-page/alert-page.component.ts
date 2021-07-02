import { Component, OnInit } from '@angular/core';
import { WebsocketConnectionService } from 'src/app/core/websocket-connection.service';

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
