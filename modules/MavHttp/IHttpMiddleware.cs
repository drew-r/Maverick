using System;
using System.Net;
namespace MavHttp
{
    public delegate void HttpMiddlewareCallback();
    public interface IHttpMiddleware
    {
        string Method { get; }
        string Route { get; }
        void Run(dynamic request, dynamic response, HttpMiddlewareCallback next);
    }
}
