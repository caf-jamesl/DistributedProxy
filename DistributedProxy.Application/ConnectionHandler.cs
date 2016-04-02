 using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using DistributedProxy.Application.Model;
using DistributedProxy.Application.Model.Enum;
using DistributedProxy.Application.Utilities;

namespace DistributedProxy.Application
{
    internal class ConnectionHandler
    {
        public static bool IsCheckingForNewHosts { get; set; }
        private static Socket ListenSocket { get; set; }
        private static Socket SendSocket { get; set; }
        private static IPEndPoint IpTcpEndPoint { get; set; }
        private readonly List<Client> _connections = new List<Client>();
        private const int PortNumber = 8044;

        internal void CheckForNewHosts()
        {
            while (IsCheckingForNewHosts)
            {
                {
                    try
                    {
                        // Will 'catch' if NO connection was pending, so statements below only occur when a connection is pending
                        var clientSocket = ListenSocket.Accept();
                        clientSocket.Blocking = false;
                        var client = new Client
                        {
                            ClientSocket = clientSocket,
                            InUse = true
                        };
                        _connections.Add(client);
                        // Let new client know current people in room
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }
            }
        }

        internal void SendNewHostSignal()
        {
            try
            {
                var endPoint = new IPEndPoint(IPAddress.Broadcast, PortNumber);
                var message = new Message { Type = MessageType.NewHost, Content = GetLocalIpAddress() };
                var bytes = SerializationHelper.ObjectToByteArray(message);
                SendSocket.SendTo(bytes, endPoint);
            }
            catch (SocketException)
            {
            }
        }

        private static string GetLocalIpAddress()
        {
            var iPHost = Dns.GetHostEntry(Dns.GetHostName());
            return (from iP in iPHost.AddressList where iP.AddressFamily == AddressFamily.InterNetwork select iP.ToString()).FirstOrDefault();
        }

        internal void SetupTcp()
        {
            try
            {
                ListenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                {
                    Blocking = false
                };
            }
            catch (SocketException)
            {
            }
            IpTcpEndPoint = new IPEndPoint(IPAddress.Any, PortNumber);
            ListenSocket.Bind(IpTcpEndPoint);
            ListenSocket.Listen(10);
        }

        internal void SetupUdp()
        {
            try
            {
                SendSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                SendSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
            }
            catch (SocketException)
            {
            }
        }
    }
}