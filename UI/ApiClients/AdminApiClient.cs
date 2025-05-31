using System;
using System.Collections.Generic;
using DTOsLibrary;
using System.Net.Http;
using System.Net.Http.Json;

namespace UI.ApiClients
{
    public class AdminApiClient
    {
        private readonly HttpClient client;
        public AdminApiClient(HttpClient client)
        {
            this.client = client;
        }
    }
}
