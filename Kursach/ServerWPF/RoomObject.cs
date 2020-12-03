using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ServerWPF
{
    public class RoomObject
    {
        private int roomId;
        TcpListener tcpListener;
        List<ClientObject> clients = new List<ClientObject>();
        List<ClientObject> blacklist = new List<ClientObject>();
        List<string> blacklistId = new List<string>();
        List<string> history = new List<string>();

        public int RoomId { get => roomId; }
        public List<ClientObject> Clients { get => clients; }
        public List<ClientObject> Blacklist { get => blacklist; }
        public List<string> BlacklistId { get => blacklistId; }
        public List<string> History { get => history; }

        public RoomObject(int port)
        {
            roomId = port;
        }

        protected internal void AddConnection(ClientObject clientObject)
        {

            if (blacklist.Contains(clientObject))
            {
                byte[] data = Encoding.Unicode.GetBytes("You are in blacklist");
                clientObject.Stream.Write(data, 0, data.Length);
            }
            else
            {
                clients.Add(clientObject);
            }
        }
        protected internal void RemoveConnection(string id)
        {

            ClientObject client = clients.FirstOrDefault(c => c.Id == id);

            if (client != null)
                clients.Remove(client);
        }

        protected internal void Listen()
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, roomId);
                tcpListener.Start();
                //MessageBox.Show($"Room {roomId} started. Waiting for connections...");

                while (true)
                {
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();

                    ClientObject clientObject = new ClientObject(tcpClient, this);
                    Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Disconnect();
            }
        }

        protected internal async void BroadcastMessage(string message, string id)
        {
            bool finish = false;
            var a = clients.ToArray();
            foreach (var item in a)
            {
                if (blacklistId.Contains(id))
                {
                    byte[] ban_data = Encoding.Unicode.GetBytes("You are banned, your messages aren't received");
                    await item.Stream.WriteAsync(ban_data, 0, ban_data.Length);
                    return;
                }
                if (message.Contains('|'+item.Id))
                {
                    byte[] private_data = Encoding.Unicode.GetBytes(message);
                    await item.Stream.WriteAsync(private_data, 0, private_data.Length);
                    finish = true;
                }

            }
            if (finish)
            {
                return;
            }
            byte[] data = Encoding.Unicode.GetBytes(message);
            for (int i = 0; i < clients.Count; i++)
            {
                if (clients[i].Id != id && !blacklistId.Contains(id))
                {
                    await clients[i].Stream.WriteAsync(data, 0, data.Length);
                }
                if (!blacklistId.Contains(id))
                {
                    history.Add(message);
                    using (StreamWriter sw = File.AppendText($@"Rooms\room{roomId}.txt"))
                    {
                        sw.WriteLine(message);
                    }
                }
            }
        }

        internal void BlacklistList()
        {
            var banlist = string.Empty;
            foreach (var item in blacklist)
            {
                banlist += $"{item.UserName}   |   {item.Id}\n";
            }
            MessageBox.Show(banlist);
        }

        protected internal void Disconnect()
        {
            tcpListener.Stop();

            for (int i = 0; i < clients.Count; i++)
            {
                clients[i].Close();
            }
            Environment.Exit(0);
        }
        public void BlacklistAdd(string id)
        {
            var a = clients.ToArray();
            foreach (var item in a)
            {
                if (item.Id == id && !blacklist.Contains(item))
                {
                    blacklist.Add(item);
                    blacklistId.Add(item.Id);
                }
            }
            
        }
        public void BlacklistRemove(string id)
        {
            foreach (var item in clients)
            {
                if (item.Id == id && blacklist.Contains(item))
                {
                    try
                    {
                        blacklist.Remove(item);
                        blacklistId.Remove(item.Id);
                    }
                    catch { }
                }
            }
        }
    }
}
