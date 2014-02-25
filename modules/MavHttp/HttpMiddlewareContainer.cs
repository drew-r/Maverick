using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;

using System.ComponentModel;
namespace MavHttp
{
    public delegate void HttpMiddlewareDelegate(dynamic request, dynamic response, HttpMiddlewareCallback next);
    
    public class HttpMiddlewareContainer : IHttpMiddleware
    {
        public HttpMiddlewareContainer(HttpMiddlewareDelegate middlewareFunc,string route,string method)
        {
            _middlewareFunc = middlewareFunc;
            Route = route;
            Method = method;
        }
        HttpMiddlewareDelegate _middlewareFunc;
        public string Route { get; private set; }
        public string Method { get; private set; }
        
        public void Run(dynamic request, dynamic response, HttpMiddlewareCallback next)
        {
            _middlewareFunc(request, response, next);                      
        }
    }
}
