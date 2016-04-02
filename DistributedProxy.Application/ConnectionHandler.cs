using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using DistributedProxy.Application.Model;
using DistributedProxy.Application.Model.Enum;
using DistributedProxy.Application.Utilities;

namespace DistributedProxy.Application
{
    internal class ConnectionHandler
    {
        public static bool IsCheckingForNewHosts { get; set; }
        private const int TcpPortNumber = 8044;
        private const int UdpPortNumber = 8045;
        private static readonly Socket TcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
        {
            Blocking = false
        };
        private static readonly Socket UdpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        private static readonly IPEndPoint IpTcpEndPoint = new IPEndPoint(IPAddress.Any, TcpPortNumber);
        private static readonly IPEndPoint UdpEndPoint = new IPEndPoint(IPAddress.Any, UdpPortNumber);
        private static readonly List<Client> Connections = new List<Client>();

        internal void AcceptNewHosts()
        {
            new Task(CheckForNewHostsSignal, TaskCreationOptions.LongRunning).Start();
            while (IsCheckingForNewHosts)
            {
                try
                {
                    var clientSocket = TcpSocket.Accept();
                    clientSocket.Blocking = false;
                    var client = new Client
                    {
                        ClientSocket = clientSocket,
                        InUse = true
                    };
                    Connections.Add(client);
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }

        internal void SetupTcp()
        {
            TcpSocket.Bind(IpTcpEndPoint);
            TcpSocket.Listen(10);
        }

        internal void SetupUdp()
        {
            try
            {
                UdpSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
                UdpSocket.Bind(UdpEndPoint);
            }
            catch (SocketException)
            {
            }
        }

        internal void SendNewHostSignal()
        {
            try
            {
                var endPoint = new IPEndPoint(IPAddress.Broadcast, UdpPortNumber);
                var message = new Message { Type = MessageType.NewHost, Content = GetLocalIpAddress() };
                var bytes = SerializationHelper.ObjectToByteArray(message);
                UdpSocket.SendTo(bytes, endPoint);
                DoAzureHack();
            }
            catch (SocketException)
            {
            }
        }

        private static void CheckForNewHostsSignal()
        {
            while (IsCheckingForNewHosts)
            {
                EndPoint localEndPoint = UdpEndPoint;
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

        private static void DoAzureHack()
        {
            try
            {
                var endPoint = new IPEndPoint(IPAddress.Parse("10.0.0.4"), UdpPortNumber);
                var message = new Message { Type = MessageType.NewHost, Content = GetLocalIpAddress() };
                var bytes = SerializationHelper.ObjectToByteArray(message);
                UdpSocket.SendTo(bytes, endPoint);
                endPoint = new IPEndPoint(IPAddress.Parse("10.0.0.5"), UdpPortNumber);
                UdpSocket.SendTo(bytes, endPoint);
            }
            catch (SocketException)
            {
            }
        }

        private static void TcpConnect(string iP)
        {
            if (iP == GetLocalIpAddress())
            {
                return;
            }
            var destinationIp = IPAddress.Parse(iP);
            var endPoint = new IPEndPoint(destinationIp, TcpPortNumber);
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

        private static string GetLocalIpAddress()
        {
            var iPHost = Dns.GetHostEntry(Dns.GetHostName());
            return (from iP in iPHost.AddressList where iP.AddressFamily == AddressFamily.InterNetwork select iP.ToString()).FirstOrDefault();
        }
    }
}