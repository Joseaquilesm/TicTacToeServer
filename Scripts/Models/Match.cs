using System;
using System.Collections.Generic;
using System.Text;
using DarkRift.Server;

namespace Scripts.Models
{
    class Match
    {
        private static ushort globalID = 0;

        public ushort id;
        private Player p1;
        private Player p2;

        public Match(Player p1, Player p2)
        {
            globalID++;
            id = globalID;

            this.p1 = p1;
            this.p2 = p2;

        }
        
        public bool PlayerSpaceClicked(ushort space, IClient client)
        {
            if(p1.Client == client)
            {
                Console.WriteLine("Player 1 clicked: " + space);
            }
            else if(p2.Client == client)
            {
                Console.WriteLine("Player 2 clicked: " + space);
            }
            else
            {
                // If there's someone whose client id is none of the players in a match
                // return false
                return false;
            }

            return true;
        }
    }
}
