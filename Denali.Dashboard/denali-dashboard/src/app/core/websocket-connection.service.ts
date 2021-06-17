import { Injectable } from '@angular/core';
import { Store } from '@ngrx/store';
import { AppState } from '../store/store.state';

@Injectable({
  providedIn: 'root'
})
export class WebsocketConnectionService {
  private static socket: WebSocket ;
  private static url: string = 'ws://localhost:5000/api/websocket'

  constructor(private store: Store<AppState>) { }

  public EnsureConnected() {
  
    let socket = WebsocketConnectionService.socket;
    if(!socket)
    {
      WebsocketConnectionService.socket = new WebSocket(WebsocketConnectionService.url);
    }
    else
    {
      switch (socket.readyState) {
        case socket.OPEN:
            console.log('Socket already open');
          break;
        case socket.CLOSED:
          socket = new WebSocket(WebsocketConnectionService.url);
          break;
  
        default:
          break;
      } 


    }

  }

  GetWebSocket() : WebSocket {
    return WebsocketConnectionService.socket;
  }
}
