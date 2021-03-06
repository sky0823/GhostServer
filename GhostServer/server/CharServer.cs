﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using GhostServer.client;
using GhostServer.constants;
using GhostServer.tools;

namespace GhostServer.server
{
    public class CharServer
    {
        private static bool isAlive;
        private static ManualResetEvent AcceptDone;

        public static List<GhostClient> Clients { get; private set; }

        public static bool IsAlive
        {
            get
            {
                return isAlive;
            }
            set
            {
                isAlive = value;

                if (!value)
                {
                    CharServer.AcceptDone.Set();
                }
            }
        }

        public static void ServerLoop()
        {
            AcceptDone = new ManualResetEvent(false);
            Clients = new List<GhostClient>();

            int PORT = ServerConstants.Char_Port;

            try
            {
                TcpListener Listener = new TcpListener(IPAddress.Any, PORT);
                Listener.Start();
                Log.Success("角色伺服器 {0} 已經上線。", Listener.LocalEndpoint);
                IsAlive = true;

                while (IsAlive)
                {
                    AcceptDone.Reset();
                    Listener.BeginAcceptSocket((iar) =>
                    {
                        new GhostClient(Listener.EndAcceptSocket(iar), ServerType.CharServer);
                        AcceptDone.Set();
                    }, null);
                    AcceptDone.WaitOne();
                }

                Listener.Stop();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                Console.Read();
            }
        }

        public static void Stop()
        {
            IsAlive = false;
        }
    }
}
