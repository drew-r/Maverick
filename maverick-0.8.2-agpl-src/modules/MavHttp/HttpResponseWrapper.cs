//Copyright © 2013 Drew Rathbone.
//drewrathbone@gmail.com 
//
//This file is part of Maverick.
//
//Maverick is free software, you can redistribute it and/or modify it under the terms of GNU Affero General Public License 
//as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version. 
//You should have received a copy of the the GNU Affero General Public License, along with Maverick. 
//
//Maverick is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty 
//of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
//Additional permission under the GNU Affero GPL version 3 section 7: 
//If you modify this Program, or any covered work, by linking or combining it with other code, such other code is not for 
//that reason alone subject to any of the requirements of the GNU Affero GPL version 3.
//
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
