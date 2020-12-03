using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ClientWPF
{
    public class Contact
    {
        protected internal string Id { get; private set; }
        protected internal NetworkStream Stream { get; private set; }
        protected internal string userName { get; set; }

        public Contact(string id, string name)
        {
            Id = id;
            userName = name;
        }
    }
}
