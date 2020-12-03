using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        RoomObject currentRoom;
        List<RoomObject> rooms;
        List<Thread> listenThreads;
        public MainWindow()
        {
            InitializeComponent();
            rooms = new List<RoomObject>();
            listenThreads = new List<Thread>();
            if (!Directory.Exists("Rooms"))
            {
                var d = Directory.CreateDirectory("Rooms");
            }

            /*
            //int default_room = 8888;
            //rooms = new List<RoomObject>();
            //listenThreads = new List<Thread>();
            //menuRooms.Items.Add(new MenuItem() { Header = $"{default_room}" });
            //try
            //{
            //    //rooms.Add(new RoomObject(room_port));
            //    var tmp = new RoomObject(default_room);
            //    currentRoom = tmp;
            //    rooms.Add(tmp);
            //    listenThreads.Add(new Thread(new ThreadStart(rooms[rooms.Count - 1].Listen)));
            //    listenThreads[listenThreads.Count - 1].Start();
            //}
            //catch (Exception ex)
            //{
            //    rooms[rooms.Count - 1].Disconnect();
            //    Console.WriteLine(ex.Message);
            //}
            */

            LoadRooms();
            LoadAllChatHistory();
            txtRoomId.Text = $"{currentRoom.RoomId}";
            //txtBlockChatWindow.Text = $"{currentRoom.History.ToArray()}";

        }

        private void addRoom(int roomId)
        {
            try
            {
                //rooms.Add(new RoomObject(room_port));
                var tmp = new RoomObject(roomId);
                currentRoom = tmp;
                rooms.Add(tmp);
                listenThreads.Add(new Thread(new ThreadStart(rooms[rooms.Count - 1].Listen)));
                listenThreads[listenThreads.Count - 1].Start();
                SaveRooms();
                LoadChatHistory(roomId);
            }
            catch (Exception ex)
            {
                rooms[rooms.Count - 1].Disconnect();
                Console.WriteLine(ex.Message);
            }
        }

        private void menuAddRoom_Click(object sender, RoutedEventArgs e)
        {

            var dlg = new AddRoomWindow() { Title = "Add room" };
            var result = dlg.ShowDialog();
            if (result == null || result.Value == false)
            {
                return;
            }

            foreach (var item in rooms)
            {
                if (item.RoomId == dlg.roomId)
                {
                    MessageBox.Show($"The room {dlg.roomId} is already exists");
                    return;
                }
            }

            var tmp = new MenuItem() { Header = dlg.roomId };
            menuRooms.Items.Add(tmp);

            addRoom(dlg.roomId);

            //comboboxContacts.Items.Add(dlg.userName);
        }

        private void SaveRooms()
        {
            using (StreamWriter sw = new StreamWriter("rooms.txt"))
            {
                foreach (var item in rooms)
                {
                    sw.WriteLine(item.RoomId);
                }
            }
        }

        private void LoadRooms()
        {
            if (!File.Exists("rooms.txt"))
            {
                using (var f = File.Open("rooms.txt", FileMode.OpenOrCreate)) { }
            }

            using (StreamReader sr = new StreamReader("rooms.txt"))
            {
                rooms = new List<RoomObject>();
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    rooms.Add(new RoomObject(Convert.ToInt32(line)));
                    try
                    {
                        listenThreads.Add(new Thread(new ThreadStart(rooms[rooms.Count - 1].Listen)));
                        listenThreads[listenThreads.Count - 1].Start();
                        currentRoom = rooms[rooms.Count - 1];
                        var tmp = new MenuItem() { Header = line };
                        tmp.Click += Tmp_Click;
                        menuRooms.Items.Add(tmp);
                    }
                    catch (Exception ex)
                    {
                        rooms[rooms.Count - 1].Disconnect();
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }

        private void Tmp_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in rooms)
            {
                if (item.RoomId == Convert.ToInt32((sender as MenuItem).Header))
                {
                    currentRoom = item;
                    txtRoomId.Text = item.RoomId.ToString();
                }
            }
        }

        private void LoadChatHistory(int roomId)    
        {
            foreach (var item in rooms)
            {
                if (item.RoomId == roomId)
                {
                    if (!File.Exists($@"Rooms\room{item.RoomId}.txt"))
                    {
                        using (var f = File.Create($@"Rooms\room{item.RoomId}.txt")) { };
                    }
                    using (StreamReader sr = new StreamReader($@"Rooms\room{item.RoomId}.txt"))
                    {
                        while (!sr.EndOfStream)
                        {
                            item.History.Add(sr.ReadLine());
                        }
                    }
                }
            }
        }

        private void LoadAllChatHistory()
        {
            foreach (var item in rooms)
            {
                if (!File.Exists($@"Rooms\room{item.RoomId}.txt"))
                {
                    using (var f = File.Create($@"Rooms\room{item.RoomId}.txt")) { }
                }
                using (StreamReader sr = new StreamReader($@"Rooms\room{item.RoomId}.txt"))
                {
                    while (!sr.EndOfStream)
                    {
                        item.History.Add(sr.ReadLine());
                    }
                }
            }
        }
    

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            var t = $"Online users in chat = {currentRoom.Clients.Count}\n";
            for (int i = 0; i < currentRoom.Clients.Count; i++)
            {
                t += ("\n" + currentRoom.Clients[i].UserName + "   |   " + currentRoom.Clients[i].Id);
            }
            MessageBox.Show(t);
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            var command = txtChat.Text.Split(' ');
            switch (command[0])
            {
                case "/ban":
                    currentRoom.BlacklistAdd(command[1]);
                    break;
                case "/unban":
                    currentRoom.BlacklistRemove(command[1]);
                    break;
                case "/banlist":
                    {
                        var b = string.Empty;
                        foreach (var item in currentRoom.Blacklist)
                        {
                            b += item.UserName + "|" + item.Id + "\r\n";
                        }
                        MessageBox.Show(b);
                    }
                    break;
                default:
                    break;
            }
            txtChat.Text = string.Empty;
        }

        private void menuCommands_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Ban users: /ban userId\n" +
                "Unban users: /unban userId\n");
        }
    }
}
