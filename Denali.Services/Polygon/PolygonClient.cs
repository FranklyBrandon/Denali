using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Denali.Services.Polygon
{
    public class PolygonClient
    {
        private readonly HttpClient _httpClient;

        public PolygonClient(HttpClient httpClient)
        {
            this._httpClient = httpClient;
        }
    }
}
