using Maverick;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using Maverick;

namespace MavHttp
{
    public class Http
    {
        [MaverickAssemblyInitializer]
        public static void AssemblyInitialize()
        {            
            ServiceLocator.MapType<IHttpDispatcher, HttpDispatcher>();
        }

        public Http()
        {            
            _interface = new HttpInterface();            
        }

        
        HttpInterface _interface;

        public bool ExceptionResponseEnabled
        {
            get { return _interface.ExceptionResponseEnabled; }
            set { _interface.ExceptionResponseEnabled = value; }
        }

        public void use(HttpMiddlewareDelegate middleware)
        {
            use(null, null,middleware);
        }

        public void use(string method, HttpMiddlewareDelegate middleware)
        {
            use(null, method, middleware);
        }

        public void use(string route, string method, HttpMiddlewareDelegate middleware)
        {
            
            use(new HttpMiddlewareContainer(middleware, route, method));
        }
        public void use(IHttpMiddleware middleware)
        {
            _interface.ContextDispatcher.AddMiddleware(middleware);            
        }

        public void listen(string uriPrefixes)
        {
            _interface.Listen(uriPrefixes.Split(','));
        }
        public void stop()
        {
            _interface.Stop();
        }
        public void GET(string route, HttpMiddlewareDelegate handler)
        {
            use(route, "GET", handler);
        }
        public void GET(HttpMiddlewareDelegate handler)
        {
            GET(null, handler);
        }
        public void POST(string route, HttpMiddlewareDelegate handler)
        {
            use(route, "POST", handler);
        }
        public void POST(HttpMiddlewareDelegate handler)
        {
            POST(null, handler);
        }
        public void PUT(string route, HttpMiddlewareDelegate handler)
        {
            use(route, "PUT", handler);
        }
        public void PUT(HttpMiddlewareDelegate handler)
        {
            use(null, handler);
        }
        public void DELETE(string route, HttpMiddlewareDelegate handler)
        {
            use(route, "DELETE", handler);
        }
        public void DELETE(HttpMiddlewareDelegate handler)
        {
            DELETE(null, handler);
        }
        public void OPTIONS(string route, HttpMiddlewareDelegate handler)
        {
            use(route, "OPTIONS", handler);
        }
        public void OPTIONS(HttpMiddlewareDelegate handler)
        {
            OPTIONS(null, handler);
        }
 
    }
}
