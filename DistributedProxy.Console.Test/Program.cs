﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DistributedProxy.Console.Test
{
    public class Program
    {
        private static void Main(string[] args)
        {
            var listener = new HttpListener();
            listener.Prefixes.Add("http://127.0.0.1:7777/");
            listener.Start();
            System.Console.WriteLine("Listening...");
            while (true)
            {
                var ctx = listener.GetContext();
                new Thread(new Relay(ctx).ProcessRequest).Start();
            }
        }
    }

    public class Relay
    {
        private readonly HttpListenerContext originalContext;

        public Relay(HttpListenerContext originalContext)
        {
            this.originalContext = originalContext;
        }

        public void ProcessRequest()
        {
            string rawUrl = originalContext.Request.RawUrl;
            ConsoleUtilities.WriteRequest("Proxy receive a request for: " + rawUrl);

            var relayRequest = (HttpWebRequest)WebRequest.Create(rawUrl);
            relayRequest.KeepAlive = false;
            relayRequest.Proxy.Credentials = CredentialCache.DefaultCredentials;
            relayRequest.UserAgent = this.originalContext.Request.UserAgent;

            var requestData = new RequestState(relayRequest, originalContext);
            relayRequest.BeginGetResponse(ResponseCallBack, requestData);
        }

        private static void ResponseCallBack(IAsyncResult asynchronousResult)
        {
            var requestData = (RequestState)asynchronousResult.AsyncState;
            ConsoleUtilities.WriteResponse("Proxy receive a response from " + requestData.context.Request.RawUrl);

            using (var responseFromWebSiteBeingRelayed = (HttpWebResponse)requestData.webRequest.EndGetResponse(asynchronousResult))
            {
                using (var responseStreamFromWebSiteBeingRelayed = responseFromWebSiteBeingRelayed.GetResponseStream())
                {
                    var originalResponse = requestData.context.Response;

                    if (responseFromWebSiteBeingRelayed.ContentType.Contains("text/html"))
                    {
                        var reader = new StreamReader(responseStreamFromWebSiteBeingRelayed);
                        string html = reader.ReadToEnd();
                        //Here can modify html
                        byte[] byteArray = System.Text.Encoding.Default.GetBytes(html);
                        var stream = new MemoryStream(byteArray);
                        stream.CopyTo(originalResponse.OutputStream);
                    }
                    else
                    {
                        responseStreamFromWebSiteBeingRelayed.CopyTo(originalResponse.OutputStream);
                    }
                    originalResponse.OutputStream.Close();
                }
            }
        }
    }

    public static class ConsoleUtilities
    {
        public static void WriteRequest(string info)
        {
            System.Console.BackgroundColor = ConsoleColor.Blue;
            System.Console.ForegroundColor = ConsoleColor.White;
            System.Console.WriteLine(info);
            System.Console.ResetColor();
        }
        public static void WriteResponse(string info)
        {
            System.Console.BackgroundColor = ConsoleColor.DarkBlue;
            System.Console.ForegroundColor = ConsoleColor.Yellow;
            System.Console.WriteLine(info);
            System.Console.ResetColor();
        }
    }

    public class RequestState
    {
        public readonly HttpWebRequest webRequest;
        public readonly HttpListenerContext context;

        public RequestState(HttpWebRequest request, HttpListenerContext context)
        {
            webRequest = request;
            this.context = context;
        }
    }
}
