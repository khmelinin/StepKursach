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

namespace ClientWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IPAddress ip = IPAddress.Parse("127.0.0.1");
        private int port = 8888;
        TcpClient client;
        NetworkStream stream;

        List<Contact> contacts;
        List<Contact> groups;
        List<Contact> blacklist;
        List<string> blacklistId;

        public MainWindow()
        {
            InitializeComponent();
            contacts = new List<Contact>();
            blacklist = new List<Contact>();
            blacklistId = new List<string>();
            groups = new List<Contact>();

            LoadAll();

            // disabling
            txtChat.IsEnabled = false;
            // disabling
        }

        private void menuConnect_Click(object sender, RoutedEventArgs e)
        {

            var dlg = new ConnectionWindow() { Title = "Connect to server" };
            var result = dlg.ShowDialog();
            if (result == null || result.Value == false)
            {
                return;
            }
            if(txtUsername.Text.Contains('|') || txtUserId.Text.Contains('|'))
            {
                MessageBox.Show("You can't have '|' in your Name or Id, try again");
                return;
            }

            /*
            var tcpClient = new TcpClient(AddressFamily.InterNetwork);
            await tcpClient.ConnectAsync(dlg.IPAddress, dlg.Port);
            if (tcpClient.Connected)
            {
                client = tcpClient;
            }
            */

            ////////////////////////////////////////////// 

            client = new TcpClient();
            try
            {
                // enabling
                txtChat.IsEnabled = true;
                txtChat.Text = string.Empty;
                // enabling 

                ip = dlg.IPAddress;
                port = dlg.Port;
                client.Connect(ip.ToString(), port); //client connect
                stream = client.GetStream();

                //string message = userName;
                string message = txtUsername.Text + '|' + txtUserId.Text;
                byte[] data = Encoding.Unicode.GetBytes(message);
                stream.Write(data, 0, data.Length);

                Thread receiveThread = new Thread(new ThreadStart(ReceiveMessage));
                receiveThread.Start();
                //SendMessage();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void SendMessage()
        {

            //while (true)
            //{
                if (txtChat.IsEnabled)
                {
                    string message = txtChat.Text;
                    byte[] data = Encoding.Unicode.GetBytes(message);
                    stream.Write(data, 0, data.Length);
                }
            //}
        }
        
        async void ReceiveMessage()
        {
            while (true)
            {
                try
                {
                    byte[] data = new byte[1024];
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = await stream.ReadAsync(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (stream.DataAvailable);

                    string message = builder.ToString();
                    if (!blacklistId.Contains(message.Split('|')[1]))
                    {
                        App.Current.Dispatcher.Invoke(() => {
                            txtBlockChatWindow.Text += ("\n" + message.Split('|')[0] + message.Split('|')[2]);
                        });
                    }
                    
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message + " Connection interrupted!");
                    Disconnect();
                }
            }
        }

        void Disconnect()
        {
            if (stream != null)
                stream.Close();
            if (client != null)
                client.Close();
            Environment.Exit(0);
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            SendMessage();
            txtBlockChatWindow.Text += ("\nMe: " + txtChat.Text);
            txtChat.Text = string.Empty;
        }

        private void txtChat_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendMessage();
                txtBlockChatWindow.Text += ("\nMe: " + txtChat.Text);
                txtChat.Text = string.Empty;
            }
        }

        private void menuAddContact_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new AddContactWindow() { Title = "Add contact" };
            var result = dlg.ShowDialog();
            if (result == null || result.Value == false)
            {
                return;
            }
            var tmp = new MenuItem() { Header = dlg.userName + "|" + dlg.userId };
            tmp.Click += menuContactCopyId;

            menuContacts.Items.Add(tmp);
            contacts.Add(new Contact(dlg.userId, dlg.userName));
            //comboboxContacts.Items.Add(dlg.userName);
            SaveContacts();
        }
        private void menuRemoveContact_Click(object sender, RoutedEventArgs e)
        {
            if(MessageBox.Show("Are you sure to delete this contact?", "Remove", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var id1 = (sender as MenuItem).Header.ToString().Split('|')[1];
                var id2 = (sender as MenuItem).Header.ToString().Split('|')[0];

                var c = contacts.ToArray();
                foreach (var item in c)
                {
                    if (item.Id == id1 || item.Id == id2)
                    {
                        contacts.Remove(item);
                    }
                }

                menuContacts.Items.Remove(sender);


                //comboboxContacts.Items.Add(dlg.userName);
                SaveContacts();
            }
            
        }

        private void menuAddGroup_Click(object sender, RoutedEventArgs e)
        {
            
            var dlg = new AddGroupWindow() { Title = "Add group" };
            var result = dlg.ShowDialog();
            if (result == null || result.Value == false)
            {
                return;
            }
            var tmp = new MenuItem() { Header = dlg.groupName +'|' + dlg.groupId };
            tmp.Click += menuEnterRoom_Click;

            menuGroups.Items.Add(tmp);
            groups.Add(new Contact(dlg.groupId, dlg.groupName));
            //comboboxGroups.Items.Add(dlg.groupName);
            SaveGroups();
            
        }

        private void menuEnterRoom_Click(object sender, RoutedEventArgs e)
        {
            
            if (txtUsername.Text.Contains('|') || txtUserId.Text.Contains('|'))
            {
                MessageBox.Show("You can't have '|' in your Name or Id, try again");
                return;
            }

            try
            {
                if (stream != null)
                    stream.Close();
                if (client != null)
                    client.Close();
            }
            catch
            {

            }

            client = new TcpClient();
            try
            {
                
                // enabling
                txtChat.IsEnabled = true;
                txtChat.Text = string.Empty;
                // enabling 

                port = Convert.ToInt32((sender as MenuItem).Header.ToString().Split('|')[1]);
                client.Connect(ip.ToString(), port); //client connect
                stream = client.GetStream();

                //string message = userName;
                string message = txtUsername.Text + '|' + txtUserId.Text;
                byte[] data = Encoding.Unicode.GetBytes(message);
                stream.Write(data, 0, data.Length);

                Thread receiveThread = new Thread(new ThreadStart(ReceiveMessage));
                receiveThread.Start();
                //SendMessage();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void menuRemoveGroup_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new AddGroupWindow() { Title = "Remove group" };
            var result = dlg.ShowDialog();
            if (result == null || result.Value == false)
            {
                return;
            }

            foreach (var item in groups)
            {
                if (item.Id == dlg.groupId)
                {
                    groups.Remove(item);
                }
            }
            foreach (var item in menuGroups.Items)
            {
                try
                {
                    if ((item as MenuItem).Header.ToString().Contains(dlg.groupId))
                    {
                        menuGroups.Items.Remove(item);
                    }
                }
                catch
                {

                }
            }
        }

        private void LoadContacts()
        {
            using (StreamReader sr = new StreamReader("contacts.txt"))
            {
                while (!sr.EndOfStream) {
                    var tmpContact = sr.ReadLine();
                    contacts.Add(new Contact(tmpContact.Split('|')[0], tmpContact.Split('|')[1]));

                    var tmp = new MenuItem() { Header = tmpContact };
                    tmp.Click += menuRemoveContact_Click;
                    menuContacts.Items.Add(tmp);
                }
            }
            using (StreamReader sr = new StreamReader("blacklist.txt"))
            {
                while (!sr.EndOfStream)
                {
                    var tmpContact = sr.ReadLine();
                    blacklist.Add(new Contact(tmpContact.Split('|')[0], tmpContact.Split('|')[1]));
                    blacklistId.Add(tmpContact.Split('|')[1]);
                    var tmp = new MenuItem() { Header = tmpContact };
                    //tmp.Click += menuContactCopyId;
                    tmp.Click += menuRemoveBlacklist1;
                    menuBlacklist.Items.Add(tmp);
                }
            }
        }

        
        private void SaveContacts()
        {
            using (StreamWriter sw = new StreamWriter("contacts.txt"))
            {
                foreach (var item in contacts)
                {
                    sw.WriteLine(item.userName + '|' + item.Id);
                }
            }
            using (StreamWriter sw = new StreamWriter("blacklist.txt"))
            {
                foreach (var item in blacklist)
                {
                    sw.WriteLine(item.userName + '|' + item.Id);
                }
            }
        }

        private void LoadGroups()
        {
            using (StreamReader sr = new StreamReader("groups.txt"))
            {
                while (!sr.EndOfStream)
                {
                    var tmpContact = sr.ReadLine();
                    groups.Add(new Contact(tmpContact.Split('|')[0], tmpContact.Split('|')[1]));

                    var tmp = new MenuItem() { Header = tmpContact };
                    tmp.Click += menuEnterRoom_Click;
                    menuGroups.Items.Add(tmp);
                }
            }
        }
        private void SaveGroups()
        {
            using (StreamWriter sw = new StreamWriter("groups.txt"))
            {
                foreach (var item in groups)
                {
                    sw.WriteLine(item.userName + '|' + item.Id);
                }
            }
        }

        private void LoadProfile()
        {
            using (StreamReader sr = new StreamReader("profile.txt"))
            {
                txtUsername.Text = sr.ReadLine();
                txtUserId.Text = sr.ReadLine();
            }
        }
        private void SaveProfile()
        {
            using (StreamWriter sw = new StreamWriter("profile.txt"))
            {
                sw.WriteLine(txtUsername.Text);
                sw.WriteLine(txtUserId.Text);
            }
        }

        private void LoadAll()
        {
            if (!File.Exists("contacts.txt"))
            {
                File.Create("contacts.txt");

            }
            else if (!File.Exists("blacklist.txt"))
            {
                File.Create("blacklist.txt");
            }
            else
            {
                LoadContacts();
            }

           

            if (File.Exists("profile.txt"))
            {
                LoadProfile();
            }
            else
            {
                txtUserId.Text = Guid.NewGuid().ToString();
                SaveProfile();
            }

            if (!File.Exists("groups.txt"))
            {
                File.Create("groups.txt");
            }
            else
            {
                LoadGroups();
            }
        }

        private void menuProfileLoad_Click(object sender, RoutedEventArgs e)
        {
            LoadProfile();
        }
        private void menuProfileSave_Click(object sender, RoutedEventArgs e)
        {
            SaveProfile();
        }

        private void menuRemoveBlacklist_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new AddContactWindow() { Title = "Remove from blacklist" };
            var result = dlg.ShowDialog();
            if (result == null || result.Value == false)
            {
                return;
            }
            //var tmp = new MenuItem() { Header = dlg.userName + " " + dlg.userId };

            //menuContacts.Items.Add(tmp);
            var b = blacklist.ToArray();
            foreach (var item in b)
            {
                if(item.Id == dlg.userId)
                {
                    blacklist.Remove(item);
                }
            }
            //var tmpContact = new Contact(dlg.userId, dlg.userName);
            //if (blacklist.Contains(tmpContact))
            //{
            //    blacklist.Remove(tmpContact);
            //    blacklistId.Remove(tmpContact.Id);
            //}

            //for (int i = 0; i < menuBlacklist.Items.Count; i++)
            //{
            //    try
            //    {
            //        if (menuBlacklist.Items .ToString() == (dlg.userName + "|" + dlg.userId))
            //        {
            //            menuBlacklist.Items.Remove(item);
            //        }
            //    }
            //    catch (Exception ex)
            //    {

            //    }
            //}
            //var tmp = new MenuItem() { Header = dlg.userName + "|" + dlg.userId };
            //tmp.Click += menuContactCopyId;
            //if (menuBlacklist.Items.Contains(tmp))
            //{
            //    menuBlacklist.Items.Remove(tmp);
            //}
            ////comboboxContacts.Items.Add(dlg.userName);
            //SaveContacts();
        }
        private void menuAddBlacklist_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new AddContactWindow() { Title = "Add to blacklist" };
            var result = dlg.ShowDialog();
            if (result == null || result.Value == false)
            {
                return;
            }
            var tmp = new MenuItem() { Header = dlg.userName + "|" + dlg.userId };
            tmp.Click += menuContactCopyId;
            tmp.MouseDoubleClick += menuRemoveBlacklist1;
            menuBlacklist.Items.Add(tmp);
            blacklist.Add(new Contact(dlg.userId, dlg.userName));
            blacklistId.Add(dlg.userId);
            //comboboxBlacklist.Items.Add(dlg.userName);
            SaveContacts();
        }

        private void menuRemoveBlacklist1(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure to delete this contact from blacklist?", "Remove", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var id1 = (sender as MenuItem).Header.ToString().Split('|')[1];
                var id2 = (sender as MenuItem).Header.ToString().Split('|')[0];

                var b = blacklist.ToArray();
                foreach (var item in b)
                {
                    if (item.Id == id1 || item.Id == id2)
                    {
                        blacklist.Remove(item);
                    }
                }

                menuBlacklist.Items.Remove(sender);
                SaveContacts();
            }
        }

        private void menuContactCopyId(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText((sender as MenuItem).Header.ToString().Split('|')[1]);
        }
    }
}
    