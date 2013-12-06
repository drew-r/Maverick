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
