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
        private static Socket UdpSocket { get; set; }
        private static IPEndPoint IpTcpEndPoint { get; set; }
        private static readonly List<Client> Connections = new List<Client>();
        private const int PortNumber = 8044;

        internal void AcceptNewHosts()
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
                        Connections.Add(client);
                        // Let new client know current people in room
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }
            }
        }

        internal void CheckForNewHostsSignal()
        {
            while (IsCheckingForNewHosts)
            {
                EndPoint localEndPoint = IpTcpEndPoint;
                var receiveBuffer = new byte[1024];
                var receiveByteCount = UdpSocket.ReceiveFrom(receiveBuffer, ref localEndPoint);
                if (0 >= receiveByteCount) continue;
                var message = (Message)SerializationHelper.ByteArrayToObject(receiveBuffer);
                DealWithMessage(message);
            }
        }

        private static void DealWithMessage(Message message)
        {
            switch (message.Type)
            {
                case MessageType.NewHost:
                    TcpConnect(message.Content);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void TcpConnect(string iP)
        {
            var destinationIp = IPAddress.Parse(iP);
            var endPoint = new IPEndPoint(destinationIp, PortNumber);
            var client = new Client
            {
                ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                {
                    Blocking = false
                },
                InUse = true
            };
            client.ClientSocket.Connect(endPoint);
            Connections.Add(client);
        }

        internal void SendNewHostSignal()
        {
            try
            {
                var endPoint = new IPEndPoint(IPAddress.Broadcast, PortNumber);
                var message = new Message {Type = MessageType.NewHost, Content = GetLocalIpAddress()};
                var bytes = SerializationHelper.ObjectToByteArray(message);
                UdpSocket.SendTo(bytes, endPoint);
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
                UdpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                UdpSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
            }
            catch (SocketException)
            {
            }
        }
    }
}