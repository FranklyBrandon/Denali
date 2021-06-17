import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class WebsocketConnectionService {
  private static socket: WebSocket;
  private static url: string;

  constructor() { }

  public Connect() {
    WebsocketConnectionService.socket = new WebSocket(WebsocketConnectionService.url);
  }
}
