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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace MavHttp
{      
    public class HttpDispatcher : IHttpDispatcher
    {
        
        object _responseLock = new object();
        public void Dispatch(HttpListenerContext context)
        {
            lock (_responseLock)
            {
                Queue<IHttpMiddleware> _mwQ = new Queue<IHttpMiddleware>(_middlewareStack.Where(
                    (item) =>
                    {
                        return
                            //items where no route or method are specified
                            (item.Route == null && item.Method == null)
                            ||
                            (item.Route == null && item.Method == context.Request.HttpMethod)
                            ||
                            (Regex.IsMatch(context.Request.HttpMethod + "#" + context.Request.Url.AbsolutePath.TrimEnd('/') + "/",
                                item.Method + "#/" + Regex.Replace((item.Route.Trim('/') + "/"), ":.+?/", "[^/]+?/") + "$"));
                    }
                    ).OrderBy((i) => i.Route != null));

               
                dynamic response = new HttpResponseWrapper(context.Response);

                if (_mwQ.Count((i) => i.Route != null) == 0) 
                {
                    response.Close(String.Format("Cannot {0} {1}", context.Request.HttpMethod, context.Request.Url.AbsolutePath.TrimEnd('/') + "/"),404);
                    return;
                }

                HttpMiddlewareCallback next = null;
                dynamic request = new HttpRequestWrapper(context.Request);
                
                
                next = () =>
                {
                    if (_mwQ.Count == 0) return;
                    IHttpMiddleware mw = _mwQ.Dequeue();
                    if (mw.Route != null) { request.RouteParams = getRouteParams(mw.Route, context.Request.Url.AbsolutePath); }
                    mw.Run(request, response, next);                  
                };
                next();                
            }
        }

        

        List<IHttpMiddleware> _middlewareStack = new List<IHttpMiddleware>();
        public void AddMiddleware(IHttpMiddleware middleware)
        {
            if (middleware.Method == null && middleware.Route != null) throw new InvalidOperationException("Must specify a HTTP method for routed middleware.");
            _middlewareStack.Add(middleware);
        }

        public void RemoveMiddleware(IHttpMiddleware middleware)
        {
            _middlewareStack.Remove(middleware);
        }

        Dictionary<string, string> getRouteParams(string route, string requestAbsolutePath)
        {
            Dictionary<int,string> paramDefinitions = new Dictionary<int, string>();
            string[] parts = route.Split('/');
            string part;
            for (int i = 0; i < parts.Length; i++)
            {
                part = parts[i];
                if (!part.StartsWith(":")) continue;
                paramDefinitions.Add(i, part.TrimStart(':'));
            }
            Dictionary<string, string> routeParams = new Dictionary<string, string>();
            string[] reqRouteParts = (requestAbsolutePath.TrimStart('/').TrimEnd('/') + "/").Split('/');
            foreach (KeyValuePair<int, string> kvp in paramDefinitions)
            {
                routeParams.Add(kvp.Value, reqRouteParts[kvp.Key-1]);
            }
            return routeParams;
        }
    }
}
