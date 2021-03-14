using System;
using System.Collections.Generic;
using System.Text;
using DarkRift.Server;

namespace Scripts.Models
{
    class Player
    {
        public readonly IClient Client;
        public readonly string Name;

        public Player(IClient client, string name)
        {
            Client = client;
            Name = name;
        }
    }
}
