using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Denali.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebsocketController : ControllerBase
    {
        [HttpGet]
        public async Task GetConnect()
        {
            if (this.HttpContext.WebSockets.IsWebSocketRequest)
            {
                var webSocket = await this.HttpContext.WebSockets.AcceptWebSocketAsync();
                while (!HttpContext.RequestAborted.IsCancellationRequested)
                {
                    var response = string.Format("Hello! Time {0}", System.DateTime.Now.ToString());
                    var bytes = System.Text.Encoding.UTF8.GetBytes(response);

                    await webSocket.SendAsync(new System.ArraySegment<byte>(bytes),
                        WebSocketMessageType.Text, true, CancellationToken.None);

                    await Task.Delay(2000);
                }       
            }
        }
    }
}
