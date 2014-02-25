using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace MavHttp
{
    public interface IHttpDispatcher
    {
        void Dispatch(HttpListenerContext context);
        void AddMiddleware(IHttpMiddleware middleware);
        void RemoveMiddleware(IHttpMiddleware middleware);
    }
}
