﻿using System;
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
        private static bool IsCheckingForMessages { get; } = true;
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
            new Task(CheckForMessages, TaskCreationOptions.LongRunning).Start();
            while (IsCheckingForNewHosts)
            {
                try
                {
                    var clientSocket = TcpSocket.Accept();
                    clientSocket.Blocking = false;
                    var client = new Client
                    {
                        ClientSocket = clientSocket
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

        internal static void SendHostLeave()
        {
            var message = new Message { Type = MessageType.HostLeave, Content = GetLocalIpAddress() };
            var bytes = SerializationHelper.ObjectToByteArray(message);
            foreach (var client in Connections)
            {
                client.ClientSocket.Send(bytes);
            }
        }

        private static void CheckForNewHostsSignal()
        {
            while (IsCheckingForNewHosts)
            {
                EndPoint localEndPoint = UdpEndPoint;
                var receiveBuffer = new byte[1024];
                try
                {
                    var receiveByteCount = UdpSocket.ReceiveFrom(receiveBuffer, ref localEndPoint);
                    if (0 >= receiveByteCount) continue;
                    var message = (Message)SerializationHelper.ByteArrayToObject(receiveBuffer);
                    DealWithMessage(message);
                }
                catch
                {
                    // ignored
                }
            }
        }

        private static void CheckForMessages()
        {
            while (IsCheckingForMessages)
            {
                foreach (var client in Connections)
                {
                    var localEndPoint = (EndPoint) IpTcpEndPoint;
                    var receiveBuffer = new byte[1024];
                    try
                    {
                        var receiveByteCount = client.ClientSocket.ReceiveFrom(receiveBuffer, ref localEndPoint);
                        if (0 < receiveByteCount)
                        {
                            var message = (Message) SerializationHelper.ByteArrayToObject(receiveBuffer);
                            DealWithMessage(message);
                        }
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }
        }

        private static void DealWithMessage(Message message)
        {
            switch (message.Type)
            {
                case MessageType.NewHost:
                    TcpConnect(message.Content);
                    break;
                case MessageType.HostLeave:
                    RemoveHost(message.Content);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void DoAzureHack()
        {
            try
            {
                IPAddress ip = null;
                if (GetLocalIpAddress() == "10.0.0.4")
                {
                    ip = IPAddress.Parse("10.0.0.5");
                }
                if (GetLocalIpAddress() == "10.0.0.5")
                {
                    ip = IPAddress.Parse("10.0.0.4");
                }
                if (ip == null)
                {
                    return;
                }
                var endPoint = new IPEndPoint(ip, UdpPortNumber);
                var message = new Message { Type = MessageType.NewHost, Content = GetLocalIpAddress() };
                var bytes = SerializationHelper.ObjectToByteArray(message);
                UdpSocket.SendTo(bytes, endPoint);
            }
            catch (SocketException)
            {
            }
        }

        internal static void RemoveHost(string message)
        {
            var host =  Connections.First(client => client.Ip == message);
            host.ClientSocket.Close();
            Connections.Remove(host);
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
            };
            try
            {
                client.ClientSocket.Connect(endPoint);
                Connections.Add(client);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static string GetLocalIpAddress()
        {
            var iPHost = Dns.GetHostEntry(Dns.GetHostName());
            return (from iP in iPHost.AddressList where iP.AddressFamily == AddressFamily.InterNetwork select iP.ToString()).FirstOrDefault();
        }
    }
}