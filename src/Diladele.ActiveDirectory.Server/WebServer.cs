using Diladele.ActiveDirectory.Inspection;
using Diladele.ActiveDirectory.Config;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Diladele.ActiveDirectory.Server
{
    public class WebServer : IDisposable
    {
        public WebServer(IConfig config, IStorage storage)
        {
            // log it
            log.Info("WebServer is starting...");

            // save the storage
            _storage = storage;

            // create new listener
            _listener = new HttpListener();
            {
                // log it
                log.InfoFormat("Will listen on port {0}", config.ListenPort);

                _listener.Prefixes.Add(string.Format("http://*:{0}/", config.ListenPort));
            }
            
            // start it
            _listener.Start();

            // and create a background thread to listen to incoming requests
            ThreadPool.QueueUserWorkItem(this.ListenThreadProc);

            // log it
            log.Info("WebServer started successfully.");
        }

        private readonly HttpListener _listener;
        private readonly IStorage     _storage;


        private void ListenThreadProc(object state)
        {
            // Console.WriteLine("Webserver running...");
            try
            {
                while (_listener.IsListening)
                {
                    ThreadPool.QueueUserWorkItem(this.ProcessRequest, _listener.GetContext());
                }
            }
            catch (Exception e)
            {
                log.ErrorFormat("WebServer stopped in panic! Error: {0}", e.ToString());
            }
        }

        private void ProcessRequest(object state)
        {
            var ctx = state as HttpListenerContext;
            
            try
            {
                // log it
                log.DebugFormat("Processing new {0} request", ctx.Request.HttpMethod);

                // we only can service gets
                if("GET" != ctx.Request.HttpMethod)
                    throw new Exception(string.Format("Invalid HTTP method {0}", ctx.Request.HttpMethod));

                // process the request
                string response = "";

                if (ctx.Request.RawUrl.StartsWith("/ip/list/"))
                    response = HandleIpList();
                else if (ctx.Request.RawUrl.StartsWith("/ip/lookup/"))
                    response = HandleIpLookUp(ctx.Request.RawUrl);
                else
                    throw new Exception(string.Format("Invalid raw URL {0}", ctx.Request.RawUrl));

                // log it
                log.DebugFormat("Got successful response {0}", response);

                // pack the response
                byte[] buf = Encoding.UTF8.GetBytes(response);
                {
                    ctx.Response.Headers["Content-Type"] = "application/json";

                    ctx.Response.ContentLength64 = buf.Length;
                    ctx.Response.OutputStream.Write(buf, 0, buf.Length);
                }
            }
            catch(Exception e)
            { 
                // write to log
                log.ErrorFormat("Request failed with error {0}", e.ToString());
                
                // respond with bad request
                string str = "{" + string.Format("\"error\" : \"{0}\", \"stacktrace\": \"{1}\"", e.Message, e.StackTrace) + "}";
                byte[] buf = Encoding.UTF8.GetBytes(str);
                {
                    ctx.Response.StatusCode = 404;
                    ctx.Response.Headers["Content-Type"] = "application/json";
                    ctx.Response.ContentLength64 = buf.Length;
                    ctx.Response.OutputStream.Write(buf, 0, buf.Length);
                }
            }
            finally
            {
                // always close the stream
                ctx.Response.OutputStream.Close();
            }
        }

        private string HandleIpLookUp(string url)
        {
            // parse out the actual ip
            Match match = Regex.Match(url, "/ip/lookup/(.*)/?", RegexOptions.IgnoreCase);
            if (!match.Success)
                throw new Exception("Invalid URL in HandleIpLookUp");
        
            // copy out the ip address
            string ip_addr = match.Groups[1].Value;
            
            // and try to find it
            Address address = null;
            if (_storage.LookUp(ip_addr, out address))
            {
                // ok we were able to find the address of the user, return it
                return address.AsJson;
            }
            throw new Exception(string.Format("No such user {0}", ip_addr));
        }

        private string HandleIpList()
        {
            // this is the result
            string result = "";

            // these are active addresses in the storage
            List<Address> addresses = null;
            
            // dump them
            if(_storage.Dump(out addresses))
            {
                if(addresses != null)
                {
                    List<string> temp = new List<string>();
                    foreach(var address in addresses)
                    {
                        temp.Add(address.AsJson);
                    }

                    result = string.Join(", ", temp.ToArray());
                }
            }

            // and repack as JSON array
            return string.Format("[{0}]", result);
        }
        
        private bool _disposed = false;
        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;

                _listener.Stop();
                _listener.Close();
            }
        }

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}
