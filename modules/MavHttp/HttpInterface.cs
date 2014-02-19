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
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MavHttp
{
    public class HttpInterface
    {
        public HttpInterface()
        {
            if (!HttpListener.IsSupported) throw new NotSupportedException("HttpListener not supported. HttpListener is supported on Windows XP SP2 / Server 2003 or later.");
        }
        IHttpDispatcher _contextDispatcher;
        internal IHttpDispatcher ContextDispatcher { get { return _contextDispatcher ?? (_contextDispatcher = ServiceLocator.Resolve<IHttpDispatcher>()); } }
        enum HttpInterfaceState { Idle, Running, Stopped }
        FiniteState State = new FiniteState(HttpInterfaceState.Idle, HttpInterfaceState.Running, HttpInterfaceState.Stopped);
        HttpListener _listener = new HttpListener();
        public void Listen(params string[] uriPrefixes)
        {
            try
            {
                State.TransitionTo(HttpInterfaceState.Running);
                foreach (string uriPrefix in uriPrefixes)
                {
                    _listener.Prefixes.Add(uriPrefix); 
                }
                _listener.Start();

                Scheduler.Request();
                new Task(delegate
                {
                    while (State == HttpInterfaceState.Running)
                    {
                        IAsyncResult res = _listener.BeginGetContext(contextCallback, null);
                        res.AsyncWaitHandle.WaitOne();                        
                    }
                }).Start();                
            }
            catch (Exception e)
            {
                Stop();
                throw;
            }
        }

        IMaverickLog _log = ServiceLocator.Resolve<IMaverickLog>();

        

        private void contextCallback(IAsyncResult arg)
        {
            HttpListenerContext context = _listener.EndGetContext(arg);
            Scheduler.Enqueue(() => 
            {            
                try
                {
                    ContextDispatcher.Dispatch(context);
                }
                catch (Exception e)
                {
                    context.Response.StatusCode = 500;
                    if (ExceptionResponseEnabled) { ExceptionResponse(context.Response, e); }
                }
                finally
                {
                    //try { context.Response.Close(); }
                    //catch (Exception e) { }
                }                
            });
            
        }

        public bool ExceptionResponseEnabled = true;
        public delegate void ExceptionResponseDelegate(HttpListenerResponse response, Exception e);
        ExceptionResponseDelegate _exceptionResponse;
        public ExceptionResponseDelegate ExceptionResponse
        {
            get
            {
                return _exceptionResponse ?? (_exceptionResponse = delegate(HttpListenerResponse response, Exception e)
                    {
                        //try to close the response with an error message             
                        string msg;
                        _log.Write(msg = Utility.ExceptionToString(e));
                        try
                        {                           
                            response.Close((response.ContentEncoding ?? (response.ContentEncoding = Encoding.UTF8)).GetBytes(msg), false);
                        }
                        catch (Exception ex)
                        {
                            _log.Write("Unable to deliver error to client as client terminated connection.");
                        }
                    });
            }
            set
            {
                _exceptionResponse = value;
            }
        }

        public void Stop()
        {

            if (_listener != null) { try { _listener.Stop(); } catch (ObjectDisposedException e) { } }
            State.TransitionTo(HttpInterfaceState.Stopped);
        }

        
        void throwFatalException(Exception e)
        {
            AsyncOperationManager.CreateOperation(null).Post((x) => { throw e; }, null);
        }

    }
}
