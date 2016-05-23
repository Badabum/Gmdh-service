using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Gmdh.Service
{
    public static class HttpResponseUtils
    {
        public static HttpResponseMessage CreateResponseWithJson<T>(HttpStatusCode code, T data)
        {
            var jsonFormatter = new JsonMediaTypeFormatter();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            return new HttpResponseMessage(code)
            {
                Content = new ObjectContent(typeof(T),data,jsonFormatter,"application/json")
            };
        }
    }
}