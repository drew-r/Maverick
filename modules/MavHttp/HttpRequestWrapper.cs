using Maverick;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;

namespace MavHttp
{
    public class HttpRequestWrapper : DynamicProxy<HttpListenerRequest>
    {
        public HttpRequestWrapper(HttpListenerRequest request) : base(request)
        {
            this["QueryString"] = new Utility.LuaNameValueCollection(request.QueryString);            
            this["Headers"] = new Utility.LuaNameValueCollection(request.Headers);
            
        }

        public void BeginGetClientCertificate(AsyncCallback callback, object state)
        {
            _proxyObj.BeginGetClientCertificate(callback, state);
        }

        public void EndGetClientCertificate(IAsyncResult result)
        {
            _proxyObj.EndGetClientCertificate(result);
        }

        public System.Security.Cryptography.X509Certificates.X509Certificate2 GetClientCertificate()
        {
            return _proxyObj.GetClientCertificate();
        }

    }
}
