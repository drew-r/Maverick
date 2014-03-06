using Maverick;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;

namespace MavHttp
{
    public class HttpResponseWrapper : DynamicProxy<HttpListenerResponse>
    {
        public HttpResponseWrapper(HttpListenerResponse response) : base(response)
        {            
        }
        public void Abort()
        {
            _proxyObj.Abort();
        }

        public void AddHeader(string name, string value)
        {
            _proxyObj.AddHeader(name, value);
        }

        public void AppendCookie(Cookie cookie)
        {
            _proxyObj.AppendCookie(cookie);
        }

        public void AppendHeader(string name, string value)
        {
            _proxyObj.AppendHeader(name, value);
        }

        public void Close()
        {
            _proxyObj.Close();
        }
        public void Close(byte[] responseEntity, bool willBlock)
        {
            _proxyObj.Close(responseEntity, willBlock);
        }

        public void Close(string responseEntity)
        {
            Close((_proxyObj.ContentEncoding ?? (_proxyObj.ContentEncoding = Encoding.UTF8)).GetBytes(responseEntity), false);            
        }

        public void Close(string responseEntity, int statusCode)
        {
            _proxyObj.StatusCode = statusCode;
            Close(responseEntity);
        }
        public void Close(int statusCode)
        {
            _proxyObj.StatusCode = statusCode;
            Close();
        }


        public void CopyFrom(HttpListenerResponse templateResponse)
        {
            _proxyObj.CopyFrom(templateResponse);
        }

        public void CopyFrom(HttpResponseWrapper templateResponseWrapper)
        {
            _proxyObj.CopyFrom(templateResponseWrapper._proxyObj);
        }

        public void Redirect(string url)
        {
            _proxyObj.Redirect(url);
        }

        public void SetCookie(Cookie cookie)
        {
            _proxyObj.SetCookie(cookie);
        }


    }
}
